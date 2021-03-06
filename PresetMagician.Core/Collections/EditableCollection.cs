using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Catel.Collections;
using Catel.Data;
using PresetMagician.Core.Data;
using ModelBase = PresetMagician.Core.Data.ModelBase;

namespace PresetMagician.Core.Collections
{
    public interface IEditableCollection : INotifyPropertyChanged, INotifyCollectionChanged
    {
        event EventHandler<PropertyChangedEventArgs> ItemPropertyChanged;
    }

    public interface IEditableCollection<T> : IList<T>, IEditableCollection, IUserEditable
        where T : class, INotifyPropertyChanged
    {
    }

    public class EditableCollection<T> : FastObservableCollection<T>, IEditableCollection<T>
        where T : class, IUserEditable, INotifyPropertyChanged
    {
        private IUserEditable _originatingEditingObject;

        private int _initialCount;
        public readonly bool IsUniqueList;
        private readonly List<T> _userModifiedItems = new List<T>();
        private readonly HashSet<T> _uniqueItems = new HashSet<T>();
        private PropertyChangedEventHandler _sharedPropertyChangedEventHandler;
        private List<T> _backupValues;


        /// <summary>
        /// Defines if this model is in editing mode and causes IsUserModified to change
        /// </summary>
        public bool IsEditing;

        public bool IsUserModified { get; private set; }


        public EditableCollection()
        {
            if (!Core.UseDispatcher)
            {
                AutomaticallyDispatchChangeNotifications = false;
            }

            _sharedPropertyChangedEventHandler = ChangeNotificationWrapperOnCollectionItemPropertyChanged;
        }

        public EditableCollection(bool isUniqueList = false)
        {
            if (!Core.UseDispatcher)
            {
                AutomaticallyDispatchChangeNotifications = false;
            }

            IsUniqueList = isUniqueList;
            _sharedPropertyChangedEventHandler = ChangeNotificationWrapperOnCollectionItemPropertyChanged;
        }


        public EditableCollection(IEnumerable<T> collection, bool isUniqueList = false) : base(collection)
        {
            IsUniqueList = isUniqueList;
            _sharedPropertyChangedEventHandler = ChangeNotificationWrapperOnCollectionItemPropertyChanged;
        }


        private void ChangeNotificationWrapperOnCollectionItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var adv = e as AdvancedPropertyChangedEventArgs;

            if (!Equals(adv.OldValue, adv.NewValue))
            {
                if (IsEditing && e.PropertyName == nameof(ModelBase.IsUserModified))
                {
                    if ((bool) adv.NewValue)
                    {
                        _userModifiedItems.Add(sender as T);
                    }
                    else
                    {
                        _userModifiedItems.Remove(sender as T);
                    }

                    UpdateIsUserModifiedFlag();
                }

                ItemPropertyChanged?.Invoke(sender, adv);
            }
        }

        protected override void ClearItems()
        {
            if (IsEditing)
            {
                foreach (var item in Items)
                {
                    item.CancelEdit(this);
                    _userModifiedItems.Remove(item);
                }
            }

            base.ClearItems();
            _uniqueItems.Clear();
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            if (IsUniqueList)
            {
                if (_uniqueItems.Contains(item))
                {
                    return;
                }

                _uniqueItems.Add(item);
            }

            base.InsertItem(index, item);

            if (IsEditing)
            {
                item.BeginEdit(this);
                item.PropertyChanged += _sharedPropertyChangedEventHandler;
                if (item.IsUserModified)
                {
                    _userModifiedItems.Add(item);
                }
            }
        }

        /// <summary>
        /// Moves the item at the specified index to a new location in the collection.
        /// </summary>
        /// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
        /// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            var oldItem = this[newIndex];
            base.MoveItem(oldIndex, newIndex);
            if (!Contains(oldItem))
            {
                if (IsUniqueList)
                {
                    _uniqueItems.Remove(oldItem);
                }

                if (IsEditing)
                {
                    _userModifiedItems.Remove(oldItem);
                    oldItem.PropertyChanged -= _sharedPropertyChangedEventHandler;
                    oldItem.CancelEdit(this);
                }
            }
        }


        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            var oldItem = this[index];
            base.RemoveItem(index);
            if (!Contains(oldItem))
            {
                if (IsUniqueList)
                {
                    _uniqueItems.Remove(oldItem);
                }

                if (IsEditing)
                {
                    _userModifiedItems.Remove(oldItem);
                    oldItem.PropertyChanged -= _sharedPropertyChangedEventHandler;
                    oldItem.CancelEdit(this);
                }
            }
        }

        /// <summary>Replaces the element at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, T item)
        {
            if (IsUniqueList)
            {
                if (_uniqueItems.Contains(item))
                {
                    return;
                }

                _uniqueItems.Add(item);
            }

            var oldItem = this[index];
            base.SetItem(index, item);

            if (IsEditing)
            {
                if (item.IsUserModified)
                {
                    _userModifiedItems.Add(item);
                }

                item.BeginEdit(this);
                item.PropertyChanged += _sharedPropertyChangedEventHandler;
            }


            if (!Contains(oldItem))
            {
                if (IsUniqueList)
                {
                    _uniqueItems.Remove(oldItem);
                }

                if (IsEditing)
                {
                    _userModifiedItems.Remove(oldItem);
                    oldItem.PropertyChanged -= _sharedPropertyChangedEventHandler;
                    oldItem.CancelEdit(this);
                }
            }
        }

        public event EventHandler CollectionCountChanged;

        public new IDisposable SuspendChangeNotifications()
        {
            return SuspendChangeNotifications(SuspensionMode.None);
        }

        public new IDisposable SuspendChangeNotifications(SuspensionMode mode)
        {
            _initialCount = Count;
            return base.SuspendChangeNotifications(mode);
        }


        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (IsDirty && _initialCount != Count)
            {
                CollectionCountChanged?.Invoke(this, System.EventArgs.Empty);
            }

            if (IsEditing)
            {
                UpdateIsUserModifiedFlag();
            }

            base.OnCollectionChanged(e);
        }

        protected void UpdateIsUserModifiedFlag()
        {
            if (!IsEditing)
            {
                return;
            }

            IsUserModified = _userModifiedItems.Count != 0 ||
                             _backupValues.Count != Count ||
                             _backupValues.Except(this).Any() || this.Except(_backupValues).Any();
        }


        public void BeginEdit()
        {
            BeginEdit(null);
        }

        public void BeginEdit(IUserEditable originatingObject)
        {
            _backupValues = new List<T>(this);
            IsUserModified = false;
            IsEditing = true;
            _originatingEditingObject = originatingObject;
            _userModifiedItems.Clear();

            foreach (var i in this)
            {
                i.BeginEdit(this);
                i.PropertyChanged += _sharedPropertyChangedEventHandler;
            }
        }

        public void EndEdit()
        {
            EndEdit(null);
        }

        public void EndEdit(IUserEditable originatingObject)
        {
            if (!IsEditing || !ReferenceEquals(_originatingEditingObject, originatingObject)) return;
            IsUserModified = false;
            IsEditing = false;
            foreach (var i in this)
            {
                i.PropertyChanged -= _sharedPropertyChangedEventHandler;
                i.EndEdit(this);
            }

            _originatingEditingObject = null;
        }

        public void CancelEdit()
        {
            CancelEdit(null);
        }

        public void CancelEdit(IUserEditable originatingObject)
        {
            if (!IsEditing || !ReferenceEquals(_originatingEditingObject, originatingObject)) return;

            IsUserModified = false;
            IsEditing = false;

            foreach (var i in this.Union(_backupValues))
            {
                i.PropertyChanged -= _sharedPropertyChangedEventHandler;
                i.CancelEdit(this);
            }


            this.SynchronizeCollection(_backupValues);


            foreach (var i in this)
            {
                i.CancelEdit(this);
            }

            _originatingEditingObject = null;
        }

        /// <summary>
        /// Occurs when the <see cref="CollectionChanged"/> event occurs on the target object.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> ItemPropertyChanged;
    }
}