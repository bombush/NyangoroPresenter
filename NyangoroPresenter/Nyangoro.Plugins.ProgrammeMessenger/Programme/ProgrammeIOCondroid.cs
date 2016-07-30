using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xaml;
using System.Xml;
using System.IO;

namespace Nyangoro.Plugins.ProgrammeMessenger.Programme
{
    //@TODO: make fluent!
    class ProgrammeIOCondroid
    {
        public ProgrammeIOCondroid()
        {
        }

        public List<ProgrammeEvent> LoadEvents()
        {
            List<ProgrammeEvent>  programmeEvents = new List<ProgrammeEvent>();

            string path = Directory.GetCurrentDirectory() + "/xml/programme_condroid.xml";

            XmlTextReader reader = new XmlTextReader(path);

            ProgrammeEvent temp = null;

            while (reader.Read())
            {
                if(reader.Name == "programme")
                {
                    if (reader.IsStartElement())
                    {
                        if (temp != null)
                            programmeEvents.Add(temp);
                        temp = new ProgrammeEvent();
                    }
                }

                if (reader.Name == "title" && reader.NodeType == XmlNodeType.Element)
                {
                    //move to CDATA section
                    reader.Read();
                    if (reader.NodeType == XmlNodeType.CDATA)
                        temp.title = reader.Value;
                    else
                        throw new Exception("Invalid Condroid title node content: missing CDATA");
                }

                if (reader.Name == "start-time" && reader.NodeType == XmlNodeType.Element)
                {
                    temp.start = reader.ReadElementContentAsDateTime();
                }

                if (reader.Name == "location" && reader.NodeType == XmlNodeType.Element)
                {
                    //move to CDATA section
                    reader.Read();
                    if (reader.NodeType == XmlNodeType.CDATA)
                        temp.location = reader.Value;
                    else
                        throw new Exception("Invalid Condroid location node content: missing CDATA");
                 //   temp.location = reader.ReadInnerXml();
                }

                if (reader.Name == "author" && reader.NodeType == XmlNodeType.Element)
                {
                    //move to CDATA section
                    reader.Read();
                    if (reader.NodeType == XmlNodeType.CDATA)
                        temp.author = reader.Value;
                    else
                        throw new Exception("Invalid Condroid author node content");
                }
            }
            //add the last one
            programmeEvents.Add(temp);
            return programmeEvents;
        }

        public List<ProgrammeEvent> GetMainStageUpcomingEvents()
        {
            List<ProgrammeEvent> loadedEvents = this.LoadEvents();
            List<ProgrammeEvent> exportEvents = new List<ProgrammeEvent>();

            foreach (ProgrammeEvent evt in loadedEvents)
            {
                bool addToList = true;

                //addToList &= (evt.location == "Kinosál");

                if (addToList)
                    exportEvents.Add(evt);
            }

            return exportEvents;
        }
    }
}
