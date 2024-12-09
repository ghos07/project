using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Tags are used to store information about an object
// These tags are completely separate from Unity's tags
// One object can have multiple tags


/// <summary>
/// Context of the tag change.
/// </summary>
public readonly struct TagChangedContext
{
    /// <summary>
    /// The action that was performed on the tags.
    /// </summary>
    public readonly TagChangeType ChangeType;
    /// <summary>
    /// The tags before the change.
    /// </summary>
    public readonly Tag[] OldTags;
    /// <summary>
    /// The tags after the change.
    /// </summary>
    public readonly Tag[] NewTags;
    /// <summary>
    /// The tags that were added as a result of the change. Empty if the change was a removal.
    /// </summary>
    public readonly Tag[] AddedTags;
    /// <summary>
    /// The tags that were removed as a result of the change. Empty if the change was an addition.
    /// </summary>
    public readonly Tag[] RemovedTags;

    public TagChangedContext(TagChangeType changeType, Tag[] oldTags, Tag[] newTags)
    {
        ChangeType = changeType;
        OldTags = oldTags;
        NewTags = newTags;

        AddedTags = newTags.Except(oldTags).ToArray();
        RemovedTags = oldTags.Except(newTags).ToArray();
    }
}


/// <summary>
/// Tag manager for objects based on an enum. Allows for adding, removing, and checking tags.
/// To use string based tags, use StringTagManager.
/// Both can be used on the same object, but it is not recommended to avoid confusion and convolution.
/// </summary>
public class TagManager : MonoBehaviour
{
    [SerializeField] private List<Tag> tags = new();
    /// <summary>
    /// Called when a tag is added.
    /// </summary>
    public event System.Action<TagChangedContext> OnTagAdded;
    /// <summary>
    /// Called when a tag is removed.
    /// </summary>
    public event System.Action<TagChangedContext> OnTagRemoved;
    /// <summary>
    /// Called when tags are cleared.
    /// </summary>
    public event System.Action<TagChangedContext> OnTagsCleared;
    /// <summary>
    /// Called when tags are set.
    /// </summary>
    public event System.Action<TagChangedContext> OnTagsSet;
    /// <summary>
    /// Called when tags are changed.
    /// </summary>
    public event System.Action<TagChangedContext> OnTagChanged;


    public static implicit operator TagManager(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out TagManager tagManager))
        {
            return tagManager;
        }
        else
        {
            Debug.Log("TagManager: Object [" + gameObject + "] does not have a TagManager component, adding one now.");
            return gameObject.AddComponent<TagManager>();
        }
    }

    public static implicit operator List<Tag>(TagManager tagManager)
    {
        return tagManager.tags;
    }

    public static bool operator ==(TagManager a, TagManager b)
    {
        return a.tags.SequenceEqual(b.tags);
    }

    public static bool operator !=(TagManager a, TagManager b)
    {
        return !a.tags.SequenceEqual(b.tags);
    }

    public override bool Equals(object obj)
    {
        if (obj is TagManager other)
        {
            return tags.SequenceEqual(other.tags);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return tags.GetHashCode();
    }

    public static TagManager operator +(TagManager a, Tag b)
    {
        a.AddTag(b);
        return a;
    }

    public static TagManager operator -(TagManager a, Tag b)
    {
        a.RemoveTag(b);
        return a;
    }

    public static TagManager operator +(TagManager a, TagManager b)
    {
        a.AddTags(b);
        return a;
    }

    public static TagManager operator -(TagManager a, TagManager b)
    {
        a.RemoveTags(b);
        return a;
    }

    /// <summary>
    /// Checks if the object has a tag.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static bool HasTag(GameObject gameObject, Tag tag)
    {
        if (gameObject.TryGetComponent(out TagManager tagManager))
        {
            return tagManager.HasTag(tag);
        }
        else
        {
            Debug.Log("TagManager: Object [" + gameObject + "] does not have a TagManager component, adding one now.");
            return gameObject.AddComponent<TagManager>().HasTag(tag);
        }
    }

    /// <summary>
    /// Checks if the object has a tag.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static bool HasTag(Collider collider, Tag tag)
    {
        if (collider.gameObject.TryGetComponent(out TagManager tagManager))
        {
            return tagManager.HasTag(tag);
        }
        else
        {
            Debug.Log("TagManager: Object [" + collider.gameObject + "] does not have a TagManager component, adding one now.");
            return collider.gameObject.AddComponent<TagManager>().HasTag(tag);
        }
    }

    /// <summary>
    /// Checks if the object has a tag.
    /// </summary>
    /// <param name="tagManager"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static bool HasTag(TagManager tagManager, Tag tag)
    {
        return tagManager.HasTag(tag);
    }

    /// <summary>
    /// Checks if the object has all tags.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    public static bool HasAllTags(GameObject gameObject, params Tag[] tags)
    {
        if (gameObject.TryGetComponent(out TagManager tagManager))
        {
            return tagManager.HasAllTags(tags);
        }
        else
        {
            Debug.Log("TagManager: Object [" + gameObject + "] does not have a TagManager component, adding one now.");
            return gameObject.AddComponent<TagManager>().HasAllTags(tags);
        }
    }

    /// <summary>
    /// Checks if the object has all tags.
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    public static bool HasAllTags(Collider collider, params Tag[] tags)
    {
        if (collider.gameObject.TryGetComponent(out TagManager tagManager))
        {
            return tagManager.HasAllTags(tags);
        }
        else
        {
            Debug.Log("TagManager: Object [" + collider.gameObject + "] does not have a TagManager component, adding one now.");
            return collider.gameObject.AddComponent<TagManager>().HasAllTags(tags);
        }
    }

    /// <summary>
    /// Checks if the object has all tags.
    /// </summary>
    /// <param name="tagManager"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    public static bool HasAllTags(TagManager tagManager, params Tag[] tags)
    {
        return tagManager.HasAllTags(tags);
    }

    /// <summary>
    /// Checks if the object has any tags.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    public static bool HasAnyTags(GameObject gameObject, params Tag[] tags)
    {
        if (gameObject.TryGetComponent(out TagManager tagManager))
        {
            return tagManager.HasAnyTags(tags);
        }
        else
        {
            Debug.Log("TagManager: Object [" + gameObject + "] does not have a TagManager component, adding one now.");
            return gameObject.AddComponent<TagManager>().HasAnyTags(tags);
        }
    }

    /// <summary>
    /// Checks if the object has any tags.
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    public static bool HasAnyTags(Collider collider, params Tag[] tags)
    {
        if (collider.gameObject.TryGetComponent(out TagManager tagManager))
        {
            return tagManager.HasAnyTags(tags);
        }
        else
        {
            Debug.Log("TagManager: Object [" + collider.gameObject + "] does not have a TagManager component, adding one now.");
            return collider.gameObject.AddComponent<TagManager>().HasAnyTags(tags);
        }
    }

    /// <summary>
    /// Checks if the object has any tags.
    /// </summary>
    /// <param name="tagManager"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    public static bool HasAnyTags(TagManager tagManager, params Tag[] tags)
    {
        return tagManager.HasAnyTags(tags);
    }

    /// <summary>
    /// Checks if the object has all tags.
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    public bool HasAllTags(params Tag[] tags)
    {
        return tags.All(tag => this.tags.Contains(tag));
    }

    /// <summary>
    /// Checks if the object has any tags.
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    public bool HasAnyTags(params Tag[] tags)
    {
        return this.tags.Intersect(tags).Any();
    }

    /// <summary>
    /// Checks if the object has a tag.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool HasTag(Tag tag)
    {
        return tags.Contains(tag);
    }

    /// <summary>
    /// Adds a tag to the object.
    /// </summary>
    /// <param name="tag"></param>
    public void AddTag(Tag tag)
    {
        if (!tags.Contains(tag))
        {
            Tag[] oldTags = tags.ToArray();
            tags.Add(tag);
            Tag[] newTags = tags.ToArray();
            OnTagAdded?.Invoke(new TagChangedContext(TagChangeType.Added, oldTags, newTags));
            OnTagChanged?.Invoke(new TagChangedContext(TagChangeType.Added, oldTags, newTags));
        }
    }

    /// <summary>
    /// Adds multiple tags to the object.
    /// </summary>
    public void AddTags(params Tag[] tags)
    {
        foreach (Tag tag in tags)
        {
            AddTag(tag);
        }
    }

    /// <summary>
    /// Removes a tag from the object.
    /// </summary>
    /// <param name="tag"></param>
    public void RemoveTag(Tag tag)
    {
        if (tags.Contains(tag))
        {
            Tag[] oldTags = tags.ToArray();
            tags.Remove(tag);
            Tag[] newTags = tags.ToArray();
            OnTagRemoved?.Invoke(new TagChangedContext(TagChangeType.Removed, oldTags, newTags));
            OnTagChanged?.Invoke(new TagChangedContext(TagChangeType.Removed, oldTags, newTags));
        }
    }

    /// <summary>
    /// Removes multiple tags from the object.
    /// </summary>
    /// <param name="tags"></param>
    public void RemoveTags(params Tag[] tags)
    {
        foreach (Tag tag in tags)
        {
            RemoveTag(tag);
        }
    }

    /// <summary>
    /// Removes all tags from the object that are present on another object.
    /// </summary>
    /// <param name="other"></param>
    public void RemoveTags(TagManager other)
    {
        RemoveTags(other.tags.ToArray());
    }

    /// <summary>
    /// Removes all tags from the object that are present on another object.
    /// </summary>
    /// <param name="other"></param>
    public void RemoveTags(GameObject other)
    {
        if (other.TryGetComponent(out TagManager otherTagManager))
        {
            RemoveTags(otherTagManager);
        }
        else
        {
            Debug.Log("TagManager: Object [" + other + "] does not have a TagManager component, adding one now.");
            RemoveTags(other.AddComponent<TagManager>());
        }
    }

    /// <summary>
    /// Clears all tags from the object.
    /// </summary>
    public void ClearTags()
    {
        if (tags.Count > 0)
        {
            Tag[] oldTags = tags.ToArray();
            tags.Clear();
            Tag[] newTags = tags.ToArray();
            OnTagsCleared?.Invoke(new TagChangedContext(TagChangeType.Cleared, oldTags, newTags));
            OnTagChanged?.Invoke(new TagChangedContext(TagChangeType.Cleared, oldTags, newTags));
        }
    }

    /// <summary>
    /// Sets the tags of the object.
    /// </summary>
    /// <param name="newTags"></param>
    public void SetTags(params Tag[] newTags)
    {
        Tag[] oldTags = tags.ToArray();
        tags = new List<Tag>(newTags);
        OnTagsSet?.Invoke(new TagChangedContext(TagChangeType.Set, oldTags, newTags));
        OnTagChanged?.Invoke(new TagChangedContext(TagChangeType.Set, oldTags, newTags));
    }

    /// <summary>
    /// Toggles a tag on the object. Applies regardless of whether the tag was ever present.
    /// </summary>
    /// <param name="tag"></param>
    public void ToggleTag(Tag tag)
    {
        if (tags.Contains(tag))
        {
            RemoveTag(tag);
        }
        else
        {
            AddTag(tag);
        }
    }

    /// <summary>
    /// Toggles multiple tags on the object. Applies regardless of whether the tags were ever present.
    /// </summary>
    /// <param name="tags"></param>
    public void ToggleTags(params Tag[] tags)
    {
        foreach (Tag tag in tags)
        {
            ToggleTag(tag);
        }
    }

    /// <summary>
    /// Sets a tag on the object. Adds the tag if value is true, removes it if value is false.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    public void SetTag(Tag tag, bool value)
    {
        if (value)
        {
            AddTag(tag);
        }
        else
        {
            RemoveTag(tag);
        }
    }

    /// <summary>
    /// Clears all tags from the object and copies the tags from another object.
    /// </summary>
    /// <param name="other"></param>
    public void SetTags(TagManager other)
    {
        SetTags(other.tags.ToArray());
    }

    /// <summary>
    /// Adds all tags from another object to this object.
    /// </summary>
    /// <param name="other"></param>
    public void AddTags(TagManager other)
    {
        AddTags(other.tags.ToArray());
    }

    /// <summary>
    /// Clears all tags from the object and copies the tags from another object.
    /// </summary>
    public void SetTags(GameObject other)
    {
        if (other.TryGetComponent(out TagManager otherTagManager))
        {
            SetTags(otherTagManager);
        }
        else
        {
            Debug.Log("TagManager: Object [" + other + "] does not have a TagManager component, adding one now.");
            SetTags(other.AddComponent<TagManager>());
        }
    }


    /// <summary>
    /// Adds all tags from another object to this object.
    /// </summary>
    public void AddTags(GameObject other)
    {
        if (other.TryGetComponent(out TagManager otherTagManager))
        {
            AddTags(otherTagManager);
        }
        else
        {
            Debug.Log("TagManager: Object [" + other + "] does not have a TagManager component, adding one now.");
            AddTags(other.AddComponent<TagManager>());
        }
    }

    public static TagManager GetTagManager(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out TagManager tagManager))
        {
            return tagManager;
        }
        else
        {
            Debug.Log("TagManager: Object [" + gameObject + "] does not have a TagManager component, adding one now.");
            return gameObject.AddComponent<TagManager>();
        }
    }
}

/// <summary>
/// Context of the tag change.
/// </summary>
public readonly struct StringTagChangedContext
{
    public readonly TagChangeType ChangeType;
    public readonly string[] OldTags;
    public readonly string[] NewTags;
    public readonly string[] AddedTags;
    public readonly string[] RemovedTags;

    public StringTagChangedContext(TagChangeType changeType, string[] oldTags, string[] newTags)
    {
        ChangeType = changeType;
        OldTags = oldTags;
        NewTags = newTags;
        AddedTags = newTags.Except(oldTags).ToArray();
        RemovedTags = oldTags.Except(newTags).ToArray();
    }
}

/// <summary>
/// Tag manager for objects based on strings. Allows for adding, removing, and checking tags.
/// To use enum based tags, use TagManager.
/// Both can be used on the same object, but it is not recommended to avoid confusion and convolution.
/// </summary>
public class StringTagManager : MonoBehaviour
{
    [SerializeField] private List<string> tags = new();
    public event System.Action<StringTagChangedContext> OnTagAdded;
    public event System.Action<StringTagChangedContext> OnTagRemoved;
    public event System.Action<StringTagChangedContext> OnTagsCleared;
    public event System.Action<StringTagChangedContext> OnTagsSet;
    public event System.Action<StringTagChangedContext> OnTagChanged;

    public bool HasTag(string tag) => tags.Contains(tag);

    public bool HasAllTags(params string[] tags) => tags.All(tag => this.tags.Contains(tag));

    public bool HasAnyTags(params string[] tags) => this.tags.Intersect(tags).Any();

    public void AddTag(string tag)
    {
        if (!tags.Contains(tag))
        {
            var oldTags = tags.ToArray();
            tags.Add(tag);
            var newTags = tags.ToArray();
            var context = new StringTagChangedContext(TagChangeType.Added, oldTags, newTags);
            OnTagAdded?.Invoke(context);
            OnTagChanged?.Invoke(context);
        }
    }

    public void AddTags(params string[] tags)
    {
        foreach (var tag in tags)
        {
            AddTag(tag);
        }
    }

    public void RemoveTag(string tag)
    {
        if (tags.Contains(tag))
        {
            var oldTags = tags.ToArray();
            tags.Remove(tag);
            var newTags = tags.ToArray();
            var context = new StringTagChangedContext(TagChangeType.Removed, oldTags, newTags);
            OnTagRemoved?.Invoke(context);
            OnTagChanged?.Invoke(context);
        }
    }

    public void RemoveTags(params string[] tags)
    {
        foreach (var tag in tags)
        {
            RemoveTag(tag);
        }
    }

    public void ClearTags()
    {
        if (tags.Count > 0)
        {
            var oldTags = tags.ToArray();
            tags.Clear();
            var context = new StringTagChangedContext(TagChangeType.Cleared, oldTags, new string[0]);
            OnTagsCleared?.Invoke(context);
            OnTagChanged?.Invoke(context);
        }
    }

    public void SetTags(params string[] newTags)
    {
        var oldTags = tags.ToArray();
        tags = new List<string>(newTags);
        var context = new StringTagChangedContext(TagChangeType.Set, oldTags, newTags);
        OnTagsSet?.Invoke(context);
        OnTagChanged?.Invoke(context);
    }

    public void ToggleTag(string tag)
    {
        if (tags.Contains(tag))
        {
            RemoveTag(tag);
        }
        else
        {
            AddTag(tag);
        }
    }

    public void ToggleTags(params string[] tags)
    {
        foreach (var tag in tags)
        {
            ToggleTag(tag);
        }
    }

    public void SetTag(string tag, bool value)
    {
        if (value)
        {
            AddTag(tag);
        }
        else
        {
            RemoveTag(tag);
        }
    }
}