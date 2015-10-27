using Actor.Base;
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

    public class bhvDynActor :actActor
    {
        public bhvDynActor()
            : base()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Console.WriteLine(asm.Location);
            Become(new bhvBehavior<string>(Do));
        }

        public void Do(string msg)
        {
            // find real assembly
            Assembly asm = Assembly.GetExecutingAssembly();
            Console.WriteLine(msg + asm.Location);
        }
    }

    public class bhvRemoteActor
    {
        private byte[] AssemblyBinaries;
        private string ActorClass;
    }

    public class bhvRemoteLoading : bhvBehavior<bhvRemoteActor> 
    {
        public bhvRemoteLoading()
        {
            this.Apply = Behavior ;
            this.Pattern = t => true ;
        }

        private void Behavior(bhvRemoteActor msg)
        {
            // download binaries
            // load assembly
            // start actor
            // send back actor name
        }
     }

    public class chunk
    {
        public int chunkPart;
        public byte[] data;
        public bool last;
        public IActor sender;
    }

    public class actActorUpload : actActor
    {
        public actActorUpload() : base(new bhvUpload()) 
        { 
        }
    }

    public class actActorDownload : actActor
    {
        public actActorDownload()
            : base(new bhvDownload())
        {
        }
    }

    public class actActorDownloadTest : actActor
    {
        public actActorDownloadTest()
            : base()
        {
            this.BecomeMany(new bhvConsole());
            // start download
            IActor down = new actActorDownload();
            // start upload
            IActor up = new actActorUpload();
            // start download
            string fileName = 
                @"..\..\..\..\Actor.Plugin\bin\x64\Debug\"+ 
                @"Actor.Plugin.dll";
            MemoryStream mem = new MemoryStream();
            FileStream str = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            try
            {
                str.CopyTo(mem);
                up.SendMessage(new Tuple<IActor, MemoryStream>(down, mem));
            } finally
            {
                str.Dispose();
            }
        }
    }

    public class bhvUpload : bhvBehavior<Tuple<IActor, MemoryStream>>
    {
        public bhvUpload()
        {
            this.Apply = Behavior;
            this.Pattern = t => true;
        }

        public void Behavior(Tuple<IActor, MemoryStream> msg)
        {

            // divide object in chunk
            int chunkSize = 2048;
            int memSize = msg.Item2.Capacity ;
            int pos = 0;
            int chunknumber = 0 ;
            List<chunk> chunkList = new List<chunk>() ;
            msg.Item2.Seek(0, SeekOrigin.Begin);
            while (pos < memSize)
            {
                chunk chk = new chunk();
                int currChunkSize = Math.Min(chunkSize, memSize - pos + 1);
                chk.chunkPart = chunknumber++;
                chk.data = new byte[currChunkSize];
                msg.Item2.Read(chk.data, 0, currChunkSize);
                pos += currChunkSize;
                chunkList.Add(chk) ;
            }
            chunkList.OrderBy(t => t.chunkPart).Last().last = true;

            foreach (var item in chunkList)
            {
                msg.Item1.SendMessage(item);
            }

        }
    }

    public class bhvDownload : bhvBehavior<chunk>
{
        private List<chunk> fChunkList = new List<chunk>() ;

        private bool fComplete = false;

        public bhvDownload()
        {
            this.Apply = Behavior ;
            this.Pattern = t => true ;
        }

        private void Behavior(chunk msg)
        {
            fChunkList.Add(msg);
            var lastMsg = fChunkList.Where(t => t.last).FirstOrDefault() ;
            if ((lastMsg != null) && (fChunkList.Count - 1 == lastMsg.chunkPart)) 
            {
                fComplete = true;
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
                Debug.Assert(Assembly.GetExecutingAssembly() != asm2) ;
                Console.WriteLine(asm2.GetName());
                Console.WriteLine("Location"+asm2.Location);

                IActor asmobj = asm2.CreateInstance("Actor.Plugin.actPlugin") as IActor;
                
                Debug.Assert(asmobj != null);
                
                // register in directory

                actDirectory.GetDirectory().Register(asmobj, "plugin");

                asmobj.SendMessage("Hello");
                actSendByName<string>.SendByName("by name", "plugin");
            }
        }
}

}
