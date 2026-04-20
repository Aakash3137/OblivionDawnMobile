
using System;
using System.Collections.Generic;

/// <summary>
/// Generic Scriptable registry that tracks entities by faction, type, and (faction × type).
/// </summary>
internal sealed class ScriptableRegistry<TEntity, TFaction, TEntityType>
    where TFaction : Enum
    where TEntityType : Enum
{
    private readonly Func<TEntity, TFaction> _getFaction;
    private readonly Func<TEntity, TEntityType> _getType;

    private readonly List<TEntity> _all = new();
    private readonly Dictionary<TFaction, List<TEntity>> _byFaction = new();
    private readonly Dictionary<TEntityType, List<TEntity>> _byType = new();
    private readonly Dictionary<(TFaction, TEntityType), List<TEntity>> _byFactionAndType = new();

    public List<TEntity> All => _all;

    public ScriptableRegistry(Func<TEntity, TFaction> getFaction, Func<TEntity, TEntityType> getType, IEnumerable<TEntityType> allTypes)
    {
        _getFaction = getFaction;
        _getType = getType;

        foreach (TFaction faction in Enum.GetValues(typeof(TFaction)))
        {
            _byFaction[faction] = new();
            foreach (var type in allTypes)
                _byFactionAndType[(faction, type)] = new();
        }

        foreach (var type in allTypes)
            _byType[type] = new();
    }

    public void Register(TEntity entity)
    {
        var faction = _getFaction(entity);
        var type = _getType(entity);
        _all.Add(entity);
        _byFaction[faction].Add(entity);
        _byType[type].Add(entity);
        _byFactionAndType[(faction, type)].Add(entity);
    }

    public void Unregister(TEntity entity)
    {
        var side = _getFaction(entity);
        var type = _getType(entity);
        _all.Remove(entity);
        _byFaction[side].Remove(entity);
        _byType[type].Remove(entity);
        _byFactionAndType[(side, type)].Remove(entity);
    }

    public List<TEntity> ByFaction(TFaction side) => _byFaction[side];
    public List<TEntity> ByType(TEntityType type) => _byType[type];
    public List<TEntity> ByFactionAndType(TFaction side, TEntityType type) => _byFactionAndType[(side, type)];
}