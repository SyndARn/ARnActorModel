using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    public static class ActorTaskFactory
    {
        private static int TaskFactory = 1;
        public static void AddActor(Action messageLoop, TaskCreationOptions taskCreationOptions)
        {
            switch (TaskFactory)
            {
                case 0: ActorTaskManyTask.AddActor(messageLoop, taskCreationOptions); break;
                default: ActorTaskLowTask.AddActor(messageLoop, taskCreationOptions); break;
            }
        }
        public static string Stat()
        {
            switch (TaskFactory)
            {
                case 0: return ActorTaskManyTask.Stat(); 
                default: return ActorTaskLowTask.Stat(); 
            }
        }
    }
}
