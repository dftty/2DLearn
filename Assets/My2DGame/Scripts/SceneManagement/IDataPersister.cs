using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyGame
{
    
    public interface IDataPersister
    {
        DataSetting GetDataSetting();

        void SetDataSetting(string dataTag, DataSetting.PersistenceType persistenceType);

        void LoadData(Data data);

        Data SaveData();
    }

    [Serializable]
    public class DataSetting
    {
        public enum PersistenceType
        {
            DoNotPersist,
            ReadOnly,
            WriteOnly,
            ReadWrite
        }

        public string dataTag = System.Guid.NewGuid().ToString();
        public PersistenceType persistenceType = PersistenceType.ReadWrite;

        public override string ToString()
        {
            return dataTag + "   " + persistenceType.ToString();
        }
    }

    public class Data
    {

    }

    public class Data<T> : Data
    {
        public T value;

        public Data(T value)
        {
            this.value = value;
        }
    }

    public class Data<T0, T1> : Data
    {
        public T0 value0;
        public T1 value1;

        public Data(T0 value0, T1 value1)
        {
            this.value0 = value0;
            this.value1 = value1;
        }
    }

    public class Data<T0, T1, T2> : Data
    {
        public T0 value0;
        public T1 value1;
        public T2 value2;

        public Data(T0 value0, T1 value1, T2 value2)
        {
            this.value0 = value0;
            this.value1 = value1;
            this.value2 = value2;
        }
    }

    public class Data<T0, T1, T2, T3> : Data
    {
        public T0 value0;
        public T1 value1;
        public T2 value2;
        public T3 value3;

        public Data(T0 value0, T1 value1, T2 value2, T3 value3)
        {
            this.value0 = value0;
            this.value1 = value1;
            this.value2 = value2;
            this.value3 = value3;
        }
    }

}