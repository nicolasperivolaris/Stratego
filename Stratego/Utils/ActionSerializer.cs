using Stratego.Model;
using Stratego.Network;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Stratego.Utils
{

    public class ActionSerializer
    {
        public Player Player;
        public ActionType ActionType;
        public Move Move;

        public ActionSerializer()
        {
        }

        public int GetSize()
        {
            return Serialize().Length;
        }

        public String Serialize()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ActionSerializer));
            using (TextWriter tw = new StringWriter())
            {
                serializer.Serialize(tw, this);
                return tw.ToString();
            }
        }

        public static ActionSerializer TryDeserialize(String data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ActionSerializer));

            try
            {
                using (TextReader reader = new StringReader(data))
                {
                        ActionSerializer result = (ActionSerializer)serializer.Deserialize(reader);
                        return result;
                }
            }
            catch (Exception)
            {
                Console.WriteLine(data);
            }

            return null;
        }
    }
}
