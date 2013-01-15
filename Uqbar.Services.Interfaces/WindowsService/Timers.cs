using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Uqbar.Services.Framework.WindowsService
{
    public class Timers
    {
        private List<TimerItem> _items = new List<TimerItem>();

        public void Add(TimerItem item)
        {
            if (_items.Contains(item) == false)
            {
                _items.Add(item);
            }
        }

        public void Remove(TimerItem item)
        {
            if (_items.Contains(item))
            {
                _items.Remove(item);
            }
        }

        public void Start(long startDelay, long period, TimerCallback callback)
        {
            Start(new TimerItem() {StartDelay = startDelay, Period = period, Callback = callback});
        }

        private void Start(TimerItem item)
        {
            Stop(item);

            Add(item);

            item.TimerInstance = new System.Threading.Timer(
                        new TimerCallback(item.Callback),
                        new AutoResetEvent(false),
                        item.StartDelay,
                        item.Period);
        }

        public TimerItem GetItem(TimerCallback callback)
        {
            return _items.FirstOrDefault(i => i.Callback == callback);
        }

        public void Stop(TimerCallback callback)
        {
            Stop(GetItem(callback));
        }

        public void Stop(TimerItem item)
        {
            if (item.TimerInstance != null)
            {
                item.TimerInstance.Dispose();
                item.TimerInstance = null;
            }
        }
    }

    public class TimerItem
    {
        public long StartDelay { get; set; }
        public long Period { get; set; }
        public TimerCallback Callback { get; set; }
        public Timer TimerInstance { get; set; }
    }
}
