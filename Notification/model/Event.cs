namespace Notification
{
    public class EventData
    {
        public long id = 0;
        public long date = 0; //Дата события в ms
        public long duration = 0; //Продолжительность события в ms
        public string name = ""; // Название события
        public string shortInfo = ""; //Краткая информация о событии
        public string info = "";  //Полная информация о событии

       
        public string kind = ""; //Тип события
        public string image = ""; //Картинка
        public string uri = ""; //Ссылка куда перейдёт
        public int kindColor= 0;//Цвет строки названия

        public virtual bool isNull() => false;

        public static EventData none = new NullEventData();


        internal bool isLocalImage()
        {
            if (image.StartsWith("http://") || image.StartsWith("https://"))
                return false;
            return true;
        }

    }
    
    public class NullEventData : EventData
    {
        override public bool isNull()
        {
            return true;
        }
    }

}

