﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
    public class Matcher<TItem,TData>
        where TItem : IItem
    {
        public List<TData> AllExternalDataItems { get; private set; }
        public List<TData> MatchedExternalDataItems { get; private set; }
        public List<TData> UnmatchedExternalDataItems { get; private set; }
        public List<TItem> AllTreeItems { get; private set; }
        public List<TItem> MatchedTreeItems { get; private set; }
        public List<TItem> UnmatchedTreeItems { get; private set; }
        List<TData> initialDataItems;
        Func<TItem, List<TData>, TData> BestMatch;
        Expression<Func<TItem, TData>> TargetField;
        Func<TItem, TData> TargetFieldLambda;
        Func<TData, TData, bool> Equals;

        public Matcher(IEnumerable<TData> allExternal, Func<TItem,List<TData>, TData> bestMatch, Expression<Func<TItem,TData>> targetField, Func<TData,TData,bool> equals)
        {
            AllExternalDataItems = new List<TData>();
            MatchedTreeItems = new List<TItem>();
            UnmatchedTreeItems = new List<TItem>();
            AllTreeItems = new List<TItem>();
            MatchedExternalDataItems = new List<TData>();
            UnmatchedExternalDataItems = new List<TData>();
            initialDataItems = allExternal.ToList();
            BestMatch = bestMatch;
            TargetField = targetField;
            TargetFieldLambda = TargetField.Compile();
            Equals = equals;
        }

        TData AccountTreeItems(TItem item)
        {
            AllTreeItems.Add(item);
            if (TargetFieldLambda(item) != null) MatchedTreeItems.Add(item);
            else UnmatchedTreeItems.Add(item);
            return TargetFieldLambda(item);
        }

        TData CheckExistingData(TItem item)
        {
            var storedData = TargetFieldLambda(item);
            if (storedData == null) return storedData;
            var foundData = UnmatchedExternalDataItems.Where(z => Equals(z, storedData)).FirstOrDefault();
            if (foundData == null) return foundData;
            UnmatchedExternalDataItems.Remove(foundData);
            MatchedExternalDataItems.Add(foundData);
            return foundData;
        }

        TData FoundNewData(TItem item)
        {
            var storedData = TargetFieldLambda(item);
            if (storedData != null) return storedData;
            var newFoundData = BestMatch(item, UnmatchedExternalDataItems);
            if (newFoundData != null)
            {
                UnmatchedExternalDataItems.Remove(newFoundData);
                MatchedExternalDataItems.Add(newFoundData);
            }
            return newFoundData;

        }

        public void Push(Item root)
        {
            AllExternalDataItems = initialDataItems.ToList();
            MatchedExternalDataItems.Clear();
            UnmatchedExternalDataItems=initialDataItems.ToList();
            AllTreeItems.Clear();
            MatchedTreeItems.Clear();
            UnmatchedTreeItems.Clear();
            DataBinding<TItem>.Pull(root, TargetField, CheckExistingData);
            DataBinding<TItem>.Pull(root, TargetField, FoundNewData);
            DataBinding<TItem>.Pull(root, TargetField, AccountTreeItems);
        }
    }
}