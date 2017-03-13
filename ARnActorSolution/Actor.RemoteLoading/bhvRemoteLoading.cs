using Actor.Base;
using Actor.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Actor.RemoteLoading
{

    public class bhvDynActor : BaseActor
    {
        public bhvDynActor()
            : base()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Console.WriteLine(asm.Location);
            Become(new Behavior<string>(Do));
        }

        public void Do(string msg)
        {
            // find real assembly
            Assembly asm = Assembly.GetExecutingAssembly();
            Console.WriteLine(msg + asm.Location);
        }
    }

    public class chunk
    {
        public int chunkPart;
        public byte[] data;
        public bool last;
        public IActor sender;
    }

    public class actActorUpload : BaseActor
    {
        public actActorUpload()
            : base(new BehaviorUpload())
        {
        }
    }

    public class actActorDownload : BaseActor
    {
        public actActorDownload()
            : base(new BehaviorDownload())
        {
        }
    }

    public class actActorDownloadTest : BaseActor
    {
        public actActorDownloadTest()
            : base()
        {
            this.Become(new ConsoleBehavior());
            // start download
            IActor down = new actActorDownload();
            // start upload
            IActor up = new actActorUpload();
            // start download
            string fileName =
                @"..\..\..\..\Actor.Plugin\bin\x64\Debug\" +
                @"Actor.Plugin.dll";
            using (MemoryStream mem = new MemoryStream())
            {
                using (FileStream str = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    str.CopyTo(mem);
                    up.SendMessage(down, mem);
                }
            }
        }
    }

    public class BehaviorUpload : Behavior<IActor, MemoryStream>
    {
        public BehaviorUpload()
        {
            this.Apply = Behavior;
            this.Pattern = (a,s) => true;
        }

        public void Behavior(IActor actor, MemoryStream stream)
        {
            CheckArg.Actor(actor);
            // divide object in chunk
            int chunkSize = 2048;
            int memSize = stream.Capacity;
            int pos = 0;
            int chunknumber = 0;
            List<chunk> chunkList = new List<chunk>();
            stream.Seek(0, SeekOrigin.Begin);
            while (pos < memSize)
            {
                chunk chk = new chunk();
                int currChunkSize = Math.Min(chunkSize, memSize - pos + 1);
                chk.chunkPart = chunknumber++;
                chk.data = new byte[currChunkSize];
                stream.Read(chk.data, 0, currChunkSize);
                pos += currChunkSize;
                chunkList.Add(chk);
            }
            chunkList.OrderBy(t => t.chunkPart).Last().last = true;

            foreach (var item in chunkList)
            {
                actor.SendMessage(item);
            }

        }
    }

    public class BehaviorDownload : Behavior<chunk>
    {
        private List<chunk> fChunkList = new List<chunk>();

        public BehaviorDownload()
        {
            this.Apply = Behavior;
            this.Pattern = t => true;
        }

        private void Behavior(chunk msg)
        {
            fChunkList.Add(msg);
            var lastMsg = fChunkList.Where(t => t.last).FirstOrDefault();
            if ((lastMsg != null) && (fChunkList.Count - 1 == lastMsg.chunkPart))
            {
                // send complete to sender
                msg.sender.SendMessage("Download complete");
                // try to do something with this assembly
                MemoryStream ms = new MemoryStream();
                Assembly asm2 = null;
                try
                {
                    foreach (var item in fChunkList.OrderBy(t => t.chunkPart))
                    {
                        ms.Write(item.data, 0, item.data.Length);
                    }

                    asm2 = Assembly.Load(ms.ToArray());
                }
                finally
                {
                    ms.Dispose();
                }
                Debug.Assert(Assembly.GetExecutingAssembly() != asm2);
                Console.WriteLine(asm2.GetName());
                Console.WriteLine("Location" + asm2.Location);

                IActor asmobj = asm2.CreateInstance("Actor.Plugin.actPlugin") as IActor;

                Debug.Assert(asmobj != null);

                // register in directory

                DirectoryActor.GetDirectory().Register(asmobj, "plugin");

                asmobj.SendMessage("Hello");
                SendByName<string>.Send("by name", "plugin");
            }
        }
    }

}
