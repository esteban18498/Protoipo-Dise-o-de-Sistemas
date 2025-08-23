using System.Collections.Generic;
using UnityEngine;


namespace ZenCommand
{
    public interface Command
    {
        void Exe();
    }

    public class Guard_Class
    {

        public List<Command> commands;

        public Guard_Class()
        {
            commands = new List<Command>();
        }

        public void Execute_Commands()
        {
            foreach (Command c in commands)
            {
                c.Exe();
            }
        }

        public void Low()
        {
            commands.Clear();
        }
    }


    namespace commands
    {

        public class Hi : Command
        {
            public void Exe()
            {
                Debug.Log("Hi");
            }
            
        }



    }
}

