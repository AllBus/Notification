using System;
using System.Xml;

namespace Notification
{
    public class ProgramProperty
    {
        public static void load()
        {
            try {
                XmlTextReader reader = new XmlTextReader("./appprop/properties.xml");
                while (reader.Read())
                {

                    // Обработка данных.
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element: // Узел является элементом.

                            if (reader.Name == "server")
                                readPropertiesServer(reader);
                            else
                            if (reader.Name == "window")
                                readPropertiesWindow(reader);

                            break;
                        case XmlNodeType.Text: // Вывести текст в каждом элементе.

                            break;
                        case XmlNodeType.EndElement: // Вывести конец элемента.

                            break;
                    }
                }

            }
            catch { }
        }

        private static void readPropertiesServer(XmlTextReader reader)
        {
            String value = reader.GetAttribute("url");
            if (value != null && value.Length > 0)
                Constants.API_URI = value;

            String period = reader.GetAttribute("period");
            if (period != null && period.Length > 0)
                long.TryParse(period, out ProgramProperty.period);
        }

        private static void readPropertiesWindow(XmlTextReader reader)
        {
            String value = reader.GetAttribute("title");
            if (value != null)
                windwowTitle = value;

            while (reader.Read())
            {

                // Обработка данных.
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // Узел является элементом.

                        if (reader.Name == "button")
                            readPropertiesButton(reader);
                      

                        break;
                    case XmlNodeType.Text: // Вывести текст в каждом элементе.

                        break;
                    case XmlNodeType.EndElement: // Вывести конец элемента.
                        return;
                        
                }
            }
        }
        private static void readPropertiesButton(XmlTextReader reader)
        {
            String id = reader.GetAttribute("id");
            if (id != null)
            {
                String value = reader.GetAttribute("clickable");
                switch (id)
                {
                    case "edit":
                        {
                            buttonEditVisible = "true" == value;
                        }
                        break;
                    case "image":
                        buttonImageClickable = "true" == value;
                        break;
                    case "author":
                        buttonAuthorVisible = "true" == value;
                        break;
                }
                   
            }
                
        }


        public static String windwowTitle = "";
        public static long period=0;

        public static bool buttonEditVisible = false;
        public static bool buttonImageClickable = false;
        public static bool buttonAuthorVisible = false;
    }
}
