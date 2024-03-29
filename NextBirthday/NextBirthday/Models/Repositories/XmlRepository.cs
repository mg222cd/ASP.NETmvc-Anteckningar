﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace NextBirthday.Models.Repositories
{
    public class XmlRepository : IRepository
    {
        private static readonly string PhysicalPath;

        private XDocument _document;

        private XDocument Document
        {
            get
            {
                return _document ?? (_document = XDocument.Load(PhysicalPath));
            }
        }

        static XmlRepository()
        {
            PhysicalPath = Path.Combine(
                AppDomain.CurrentDomain.GetData("DataDirectory").ToString(),
                "Birthdates.xml");
        }

        public IEnumerable<Birthday> GetBirthdays()
        {
            return (from birthdate in Document.Descendants("birthdate")
                    select new Birthday
                    {
                        Name = birthdate.Element("name").Value,
                        Birthdate = DateTime.Parse(birthdate.Element("date").Value)
                    }).OrderBy(b => b.DaysUntilNextBirthday).ToList();
        }

        public void InsertBirthday(Birthday birthday)
        {
            Document.Root.Add(
                new XElement("birthdate",
                        new XElement("name", birthday.Name),
                        new XElement("date", birthday.Birthdate)));
        }

        public void Save()
        {
            Document.Save(PhysicalPath);
        }
    }
}