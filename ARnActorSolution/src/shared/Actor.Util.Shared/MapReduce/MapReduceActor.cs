using System;
using System.Collections.Generic;
using System.Linq;
using Actor.Base;

namespace Actor.Util
{
    public class MapReduceActor<TData, TKeyMap, TValueMap, TKeyReduce, TValueReduce> : BaseActor
    {
        private readonly Dictionary<TKeyReduce, List<TValueReduce>> _dict = new Dictionary<TKeyReduce, List<TValueReduce>>();
        private int _activeMap;

        public MapReduceActor
            (
                Action<IActor, TData> senderKV,
                MapAction<TKeyMap, TValueMap> mapKV,
                ReduceFunction<TKeyReduce, TValueReduce> reduceKV,
                IActor outputActor
            ) : base()
        {
            // start reduce
            var bhvStart = new Behavior<TData>(d =>
            {
                _activeMap++;
                senderKV(this, d);
            });

            // receive data to process
            var bhvInput = new Behavior<TKeyMap, TValueMap>(
                (k, v) =>
                {
                    // parse data
                    var map = new MapActor<TKeyMap, TValueMap>(this, mapKV);
                    _activeMap++;
                    map.SendMessage((IActor)this, k, v);
                }
                );

            // end parse
            var bhvEndParse = new Behavior<TData, IActor>((d, a) => _activeMap--);

            // receive from Map, index
            var bhvMap2Index = new Behavior<TKeyReduce, TValueReduce>
                (
                (k, v) =>
                {
                    if (!_dict.TryGetValue(k, out List<TValueReduce> someValue))
                    {
                        _dict[k] = new List<TValueReduce>();
                    }

                    _dict[k].Add(v);
                }
                );

            // receive end of job from Map, go to reduce
            var bhvMap2EndOfJov = new Behavior<MapActor<TKeyMap, TValueMap>>
                (
                (a) =>
                    {
                        _activeMap--;
                        if (_activeMap > 0)
                        {
                            return;
                        }
                        // launch reduce
                        foreach (var item in _dict)
                        {
                            var red = new ReduceActor<TKeyReduce, TValueReduce>(this, reduceKV);
                            red.SendMessage(item.Key, item.Value.AsEnumerable());
                        }
                    }
                );

            // receive from Reduce, send to output
            var bhvReduceToOutput = new Behavior<ReduceActor<TKeyReduce, TValueReduce>, TKeyReduce, TValueReduce>
                (
                (r, k, v) => outputActor.SendMessage(k, v)
                );

            var bhvs = new Behaviors();
            bhvs.AddBehavior(bhvStart);
            bhvs.AddBehavior(bhvEndParse);
            bhvs.AddBehavior(bhvInput);
            bhvs.AddBehavior(bhvMap2Index);
            bhvs.AddBehavior(bhvMap2EndOfJov);
            bhvs.AddBehavior(bhvReduceToOutput);
            Become(bhvs);
        }
    }

    public delegate void MapAction<TKeyMap, TValueMap>(IActor actor, TKeyMap keyMap, TValueMap valueMap);

    public class MapActor<TKeyMap, TValueMap> : BaseActor
    {
        private readonly IActor _sender;
        private readonly MapAction<TKeyMap, TValueMap> _mapAction;

        public MapActor(IActor sender, MapAction<TKeyMap, TValueMap> mapAction) : base()
        {
            _sender = sender;
            _mapAction = mapAction;
            Become(new Behavior<IActor, TKeyMap, TValueMap>(DoMapAction));
        }

        private void DoMapAction(IActor act, TKeyMap key, TValueMap value)
        {
            _mapAction(_sender, key, value);
            // end of job
            _sender.SendMessage(this);
        }
    }

    public delegate TValue ReduceFunction<TKey, TValue>(TKey key, IEnumerable<TValue> values);

    public class ReduceActor<TKey, TValue> : BaseActor
    {
        private readonly IActor _sender;
        private readonly ReduceFunction<TKey, TValue> _reduceFunction;

        public ReduceActor(IActor sender, ReduceFunction<TKey, TValue> reduceFunction) : base()
        {
            _reduceFunction = reduceFunction;
            _sender = sender;
            Become(new Behavior<TKey, IEnumerable<TValue>>(DoReduceFunction));
        }

        private void DoReduceFunction(TKey key, IEnumerable<TValue> someValues)
        {
            TValue value = _reduceFunction(key, someValues);
            _sender.SendMessage(this, key, value);
        }
    }
}
