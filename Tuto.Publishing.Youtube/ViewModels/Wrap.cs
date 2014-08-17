﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing.Youtube.ViewModel
{
    public class Wrap
    {
        public int NumberInTopic { get; set; }
        public Wrap Parent { get; set; }
        public List<Wrap> Children { get; private set; }
        public Wrap() { Children = new List<Wrap>(); }
        public virtual int Digits { get { return 2; } }
        public bool Root { get; set; }
        public IEnumerable<Wrap> PathFromRoot
        {
            get
            {
                if (Parent != null)
                    foreach (var e in PathFromRoot)
                        yield return e;
                yield return this;

            }
        }
        public string FormattedNumberInTopic
        {
            get
            {
                var format = "{0:D" + Digits + "}";
                return string.Format(format, NumberInTopic + 1);
            }
        }
    }
}