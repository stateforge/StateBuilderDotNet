

namespace StateForge.Examples.Turnstile
{
    using System;

    public class Barrier
    {
        public virtual void DoLock()
        {
            Console.WriteLine("Barrier.DoLock");
        }

        public virtual void DoUnlock()
        {
            Console.WriteLine("Barrier.DoUnlock");
        }
    }

    public class Alarm
    {
        public virtual void DoRing()
        {
            Console.WriteLine("Alarm.DoRing");
        }
        public virtual void DoAlertStaff()
        {
            Console.WriteLine("Alarm.DoAlertStaff");
        }
    }

    public class CoinMachine
    {
        public virtual void DoAccept()
        {
            Console.WriteLine("CoinMachine.DoAccept");
        }

        public virtual void DoReject()
        {
            Console.WriteLine("CoinMachine.DoReject");
        }
    }

    public partial class Turnstile
    {
        private Barrier barrier;
        private Alarm alarm;
        private CoinMachine coinMachine;
        private TurnstileContext context;

        public Turnstile()
        {
            this.barrier = new Barrier();
            this.alarm = new Alarm();
            this.coinMachine = new CoinMachine();
            this.context = new TurnstileContext(barrier, alarm, coinMachine);
            this.context.EnterInitialState();
        }

        static void Main(string[] args)
        {
            Turnstile myTurnstile = new Turnstile();
            myTurnstile.Coin();
            myTurnstile.Pass();
            myTurnstile.Coin();
            myTurnstile.Pass();
            Environment.Exit(0);
        }
    }
}
