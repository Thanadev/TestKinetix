using System.Collections;

namespace TestKinetix
{
    public interface IEmote
    {
        public void Play();
        public IEnumerator Cancel(); 
    }
}