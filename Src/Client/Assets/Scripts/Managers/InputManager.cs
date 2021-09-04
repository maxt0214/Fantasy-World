using System.Collections.Generic;
using Common.Data;
using Models;
using SkillBridge.Message;
using System.Linq;

namespace Managers
{
    class InputManager : MonoSingleton<InputManager>
    {
        private List<bool> activeSources = new List<bool>();
        private int idx = 0;

        public bool InputMode
        {
            get
            {
                return activeSources.Any(s => s == true);
            }
        }

        public int RegisterInputSource()
        {
            activeSources.Add(false);
            return idx++;
        }

        public void EnableInputSource(int inputId, bool condition)
        {
            if(inputId <= idx) //Valid
            {
                activeSources[inputId] = condition;
            }
        }
    }
}
