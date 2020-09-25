using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYARTE_EVOLVI
{
    public class Screen
    {
        public string Name { get; set; }
        public Modal ScreenModal { get; set; }

        public event EventHandler Entered;

        public Screen()
        {
            ScreenModal = new Modal();
        }

        public Screen(string name)
        {
            this.Name = name;
        }

        public virtual void OnEntered(EventArgs e)
        {
            Entered?.Invoke(this, e);
        }

        public virtual void Update()
        {

        }

        public virtual void Draw()
        {

        }
    }
}
