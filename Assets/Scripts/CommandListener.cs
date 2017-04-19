using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class CommandListener : IDisposable
    {
        /// <summary>
        /// Create CommandListener instance
        /// </summary>
        /// <param name="cmd">Listened command</param>
        /// <param name="amountToTrigger">Amount to callback</param>
        /// <param name="loop">Listen endlessly</param>
        public CommandListener(string cmd, int amountToTrigger, Action callback, bool loop = false)
        {
            this.cmd = cmd;
            this.amountToTrigger = amountToTrigger;
            this.loop = loop;
            listens = true;
            currentAmount = 0;
            this.callback = callback;
        }
        private bool loop;
        private bool listens;
        private Action callback;
        private int amountToTrigger;
        private int currentAmount;
        private string cmd;
        public bool Listens
        {
            get
            {
                return listens;
            }
        }
        public int AmountToTrigger
        {
            get
            {
                return amountToTrigger;
            }
        }
        public int CurrentAmount
        {
            get
            {
                return currentAmount;
            }
            set
            {
                currentAmount += value;
                if (currentAmount >= amountToTrigger)
                {
                    callback();
                    if (loop)
                    {
                        currentAmount = 0;
                    }
                    else
                    {
                        Dispose();
                    }
                }
            }
        }
        public string Command
        {
            get
            {
                return cmd;
            }
        }
        public void Dispose()
        {
            Debug.Log("DISPOSED: <" + cmd + ">");
            listens = false;
        }
    }
}
