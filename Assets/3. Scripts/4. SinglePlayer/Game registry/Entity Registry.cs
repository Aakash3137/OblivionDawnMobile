using System;
using System.Collections.Generic;

/// <summary>
/// Generic registry that tracks entities by side, type, and (side × type).
/// </summary>
internal sealed class EntityRegistry<TEntity, TSide, TEntityType>
    where TSide : Enum
    where TEntityType : Enum
{
    private readonly Func<TEntity, TSide> _getSide;
    private readonly Func<TEntity, TEntityType> _getType;

    private readonly List<TEntity> _all = new();
    private readonly Dictionary<TSide, List<TEntity>> _bySide = new();
    private readonly Dictionary<TEntityType, List<TEntity>> _byType = new();
    private readonly Dictionary<(TSide, TEntityType), List<TEntity>> _bySideAndType = new();

    public List<TEntity> All => _all;

    public EntityRegistry(Func<TEntity, TSide> getSide, Func<TEntity, TEntityType> getType, IEnumerable<TEntityType> allTypes)
    {
        _getSide = getSide;
        _getType = getType;

        foreach (TSide side in Enum.GetValues(typeof(TSide)))
        {
            _bySide[side] = new();
            foreach (var type in allTypes)
                _bySideAndType[(side, type)] = new();
        }

        foreach (var type in allTypes)
            _byType[type] = new();
    }

    public void Register(TEntity entity)
    {
        var side = _getSide(entity);
        var type = _getType(entity);
        _all.Add(entity);
        _bySide[side].Add(entity);
        _byType[type].Add(entity);
        _bySideAndType[(side, type)].Add(entity);
    }

    public void Unregister(TEntity entity)
    {
        var side = _getSide(entity);
        var type = _getType(entity);
        _all.Remove(entity);
        _bySide[side].Remove(entity);
        _byType[type].Remove(entity);
        _bySideAndType[(side, type)].Remove(entity);
    }

    public List<TEntity> BySide(TSide side) => _bySide[side];
    public List<TEntity> ByType(TEntityType type) => _byType[type];
    public List<TEntity> BySideAndType(TSide side, TEntityType type) => _bySideAndType[(side, type)];
}