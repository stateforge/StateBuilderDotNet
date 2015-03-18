
namespace StateForge.Samples
{
    interface IHelloWorldEvent
    {
        // not compiling
        public void foo();
        void evStart();
        void evStop();
    }
}
