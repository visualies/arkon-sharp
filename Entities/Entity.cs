namespace ArkonSharp.Entities
{
    public abstract class Entity
    {
        internal ArkonSharpClient Client { get; set; }
        //internal RconConnection Connection { get; set; }
        public Entity()
        {

        }
    }
}