using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutotaskQueryService
{
    public class ResultSet : IList<IList<string>>
    {
        public class InvalidFieldCountException : Exception { }

        public static ResultSet Empty { get { return new ResultSet(Enumerable.Empty<string>()); } }

        private readonly IList<IList<string>> _rows = new List<IList<string>>();

        public IList<string> HeaderRow { get; private set; }

        public ResultSet(IEnumerable<string> header)
        {
            if (header == null)
                throw new ArgumentNullException("header");

            HeaderRow = header.ToList();
        }

        #region Public API

        public int Count { get { return _rows.Count; } }

        public bool IsReadOnly { get { return false; } }

        public void Add(IList<string> item)
        {
            if (item.Count != HeaderRow.Count)
                throw new InvalidFieldCountException();

            _rows.Add(item);
        }

        public void Clear()
        {
            _rows.Clear();
        }

        public bool Contains(IList<string> item)
        {
            return _rows.Contains(item);
        }

        public void CopyTo(IList<string>[] array, int arrayIndex)
        {
            _rows.CopyTo(array, arrayIndex);
        }

        public bool Remove(IList<string> item)
        {
            return _rows.Remove(item);
        }

        public IEnumerator<IList<string>> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        public int IndexOf(IList<string> item)
        {
            return _rows.IndexOf(item);
        }

        public void Insert(int index, IList<string> item)
        {
            _rows.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _rows.RemoveAt(index);
        }

        public IList<string> this[int index]
        {
            get { return _rows[index]; }
            set { _rows[index] = value; }
        }

        #endregion
    }
}
