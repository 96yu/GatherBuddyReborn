using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using GatherBuddy.Interfaces;

namespace GatherBuddy.AutoGather;

public class PerItemSettings(IGatherable gatherable)
{
    private IDictionary<int, PerItemRotation> _perItemRotationDictionary = new Dictionary<int, PerItemRotation>();

    public IGatherable TargetGatherable { get; } = gatherable;

    public class PerItemRotation
    {
        public OrderedActionList Actions = new();

        public uint GpRequirement { get; set; } = 0;

        public class OrderedActionList : List<OrderedAction>
        { }

        public class OrderedAction
        {
            public byte Position { get; set; }
        }
    }
}

public class PerItemConfig
{
    public bool Enabled { get; set; } = false;

    public PerItemConfigList PerItemConfigurationCollection { get; set; } = new();

}

public class PerItemConfigList : IList<PerItemSettings>
{
    private IList<PerItemSettings> _perItemConfigList;

    public IEnumerator<PerItemSettings> GetEnumerator()
        => _perItemConfigList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)_perItemConfigList).GetEnumerator();

    public void Add(PerItemSettings item)
    {
        _perItemConfigList.Add(item);
    }

    public void Clear()
    {
        _perItemConfigList.Clear();
    }

    public bool Contains(PerItemSettings item)
        => _perItemConfigList.Contains(item);

    public void CopyTo(PerItemSettings[] array, int arrayIndex)
    {
        _perItemConfigList.CopyTo(array, arrayIndex);
    }

    public bool Remove(PerItemSettings item)
        => _perItemConfigList.Remove(item);

    public void RemoveAll(Predicate<PerItemSettings> predicate)
    {
        int removeIndex = -1;
        for (int i=0; i < _perItemConfigList.Count-1; i++)
        {
            var item = _perItemConfigList[i];
            if (predicate(item))
            {
                removeIndex = i;
                break;
            }
        }

        if (removeIndex <= 0)
            _perItemConfigList.RemoveAt(removeIndex);
    }

    public int Count
        => _perItemConfigList.Count;

    public bool IsReadOnly
        => _perItemConfigList.IsReadOnly;

    public int IndexOf(PerItemSettings item)
        => _perItemConfigList.IndexOf(item);

    public void Insert(int index, PerItemSettings item)
    {
        _perItemConfigList.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _perItemConfigList.RemoveAt(index);
    }

    public PerItemSettings this[int index]
    {
        get => _perItemConfigList[index];
        set => _perItemConfigList[index] = value;
    }

    public void Add(IGatherable gatherable)
        => _perItemConfigList.Add(new PerItemSettings(gatherable));

    public bool Contains(IGatherable gatherable)
        => _perItemConfigList.Any(x => x.TargetGatherable.ItemId == gatherable.ItemId);

    public bool ContainsGatherableWithId(int gatherableId)
        => _perItemConfigList.Any(x => x.TargetGatherable.ItemId == gatherableId);

    public void RemoveWhere(Predicate<PerItemSettings> func)
        => RemoveAll(func);
}
