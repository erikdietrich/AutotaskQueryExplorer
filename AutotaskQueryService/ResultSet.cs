using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutotaskQueryService
{
    public class ResultSet : ICollection<ICollection<string>>
    {
        public class InvalidFieldCountException : Exception { }

        public static ResultSet Empty { get { return new ResultSet(Enumerable.Empty<string>()); } }

        private readonly ICollection<ICollection<string>> _rows = new Collection<ICollection<string>>();

        public ICollection<string> HeaderRow { get; private set; }

        public ResultSet(IEnumerable<string> header)
        {
            if (header == null)
                throw new ArgumentNullException("header");

            HeaderRow = header.ToList();
        }

        #region ICollection<ICollection<string>> Members

        public int Count { get { return _rows.Count; } }

        public bool IsReadOnly { get { return false; } }

        public void Add(ICollection<string> item)
        {
            if (item.Count != HeaderRow.Count)
                throw new InvalidFieldCountException();

            _rows.Add(item);
        }

        public void Clear()
        {
            _rows.Clear();
        }

        public bool Contains(ICollection<string> item)
        {
            return _rows.Contains(item);
        }

        public void CopyTo(ICollection<string>[] array, int arrayIndex)
        {
            _rows.CopyTo(array, arrayIndex);
        }

        public bool Remove(ICollection<string> item)
        {
            return _rows.Remove(item);
        }

        #endregion

        #region IEnumerable<ICollection<string>> Members

        public IEnumerator<ICollection<string>> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        #endregion
    }
}
