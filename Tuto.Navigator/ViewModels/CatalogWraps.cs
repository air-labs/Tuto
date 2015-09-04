﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Navigator
{
    public abstract class Wrap
    {
        public Wrap Parent { get; set; }
        public ObservableCollection<Wrap> Items { get; private set; }
        public Wrap()
        {
            Items=new ObservableCollection<Wrap>();
        }
        public IEnumerable<Wrap> Subtree
        {
            get
            {
                yield return this;
                foreach (var e in Items)
                    foreach (var x in e.Subtree)
                        yield return x;
            }
        }
    }

    public class TopicWrap : Wrap
    {
        public Topic Topic { get; private set; }

        public TopicWrap(Topic topic)
        {
            this.Topic = topic;
            foreach (var e in topic.Items)
            {
                var child = new TopicWrap(e);
                Items.Add(child);
                child.Parent = this;
            }
        }
    }

    public class VideoWrap : Wrap
    {
        public FinishedVideo Video { get; private set; }
        public bool Checked { get; set;}
        public VideoWrap(FinishedVideo data)
        {
            Video = data;
        }
    }
}
