using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Serialize.LiquidNET
{
    [Serializable()]
    public class PropertyMap : IEnumerable
    {
        private ArrayList _List;


        internal PropertyMap()
        {
            _List = new ArrayList();
        }



        public PropertyMap PrimaryKeyProperties
        {
            get
            {
                PropertyMap map = new PropertyMap();

                foreach (PropertyDef def in this)
                {
                    if (def.KeyType == KeyType.PrimaryKey)
                    {
                        map.AddPropertyDef(def.PropertyName, def.ColumnIndex, def.ColumnName, def.DBType, def.SystemType, def.KeyType);
                    }
                }
                return map;
            }
        }


        internal void AddPropertyDef(
            string aPropertyName,
            int aColumnIndex,
            string aColumnName)
        {
            PropertyDef def = new PropertyDef(
                                            aPropertyName,
                                            aColumnIndex,
                                            aColumnName);
            _List.Add(def);
        }

        internal void AddPropertyDef(
            string aPropertyName,
            int aColumnIndex,
            string aColumnName,
            KeyType aKeyType)
        {
            PropertyDef def = new PropertyDef(
                                            aPropertyName,
                                            aColumnIndex,
                                            aColumnName,
                                            aKeyType);
            _List.Add(def);

        }

        internal void AddPropertyDef(
            string aPropertyName,
            int aColumnIndex,
            string aColumnName,
            string aDBType,
            string aSystemType,
            KeyType aKeyType)
        {
            PropertyDef def = new PropertyDef(
                                            aPropertyName,
                                            aColumnIndex,
                                            aColumnName,
                                            aDBType,
                                            aSystemType,
                                            aKeyType);
            _List.Add(def);

        }


        internal void AddPropertyDef(
            int aColumnIndex,
            string aPropertyName,
            string aColumnName,
            string aDBType,
            string aSystemType,
            KeyType aKeyType)
        {
            PropertyDef def = new PropertyDef(
                                            aPropertyName,
                                            aColumnIndex,
                                            aColumnName,
                                            aDBType,
                                            aSystemType,
                                            aKeyType);
            _List.Add(def);

        }






        internal void AddPropertyDef(
                string aPropertyName,
                int aColumnIndex,
                string aColumnName,
                string aDBType,
                string aSystemType,
                KeyType aKeyType,
                string aFKField,
                string aFKObject,
                string aFKRelation)
        {
            PropertyDef def = new PropertyDef(
                                            aPropertyName,
                                            aColumnIndex,
                                            aColumnName,
                                            aDBType,
                                            aSystemType,
                                            aKeyType);
            
            def.FKField = aFKField;
            def.FKObject = aFKObject;
            def.FKRelation = aFKRelation;

            _List.Add(def);
        }

        internal void AddPropertyDef(
                int aColumnIndex,
                string aPropertyName,
                string aColumnName,
                string aDBType,
                string aSystemType,
                KeyType aKeyType,
                string aFKField,
                string aFKObject,
                string aFKRelation)
        {
            PropertyDef def = new PropertyDef(
                                            aPropertyName,
                                            aColumnIndex,
                                            aColumnName,
                                            aDBType,
                                            aSystemType,
                                            aKeyType);

            def.FKField = aFKField;
            def.FKObject = aFKObject;
            def.FKRelation = aFKRelation;

            _List.Add(def);
        }


        public string ColumnName(string aPropertyName)
        {
            string ret = String.Empty;
            foreach (PropertyDef def in this)
            {
                if (def.PropertyName.Equals(aPropertyName))
                {
                    ret = def.ColumnName;
                    break;
                }
            }
            return ret;
        }
        public PropertyDef GetWithColumnName(string aColumnName)
        {
            PropertyDef ret = null;
            foreach (PropertyDef def in this)
            {
                if (def.ColumnName.Equals(aColumnName))
                {
                    ret = def;
                    break;
                }
            }
            return ret;
        }

        public PropertyDef GetWithPropertyName(string aPropertyName)
        {
            PropertyDef ret = null;
            foreach (PropertyDef def in this)
            {
                //if (def.PropertyName.Equals(aPropertyName))
                if (def.ColumnName.Equals(aPropertyName))
                {
                    ret = def;
                    break;
                }
            }
            return ret;
        }

        internal PropertyDef GetRelationProperty(string aRelation)
        {
            PropertyDef ret = null;
            foreach (PropertyDef def in this)
            {
                if (def.FKRelation.Equals(aRelation))
                {
                    ret = def;
                    break;
                }
            }
            return ret;
        }

        public PropertyDef this[int aIndex]
        {
            get
            {
                return (PropertyDef)_List[aIndex];
            }
        }

        public int Count
        {
            get
            {
                return _List.Count;
            }
        }
        public IEnumerator GetEnumerator()
        {
            return _List.GetEnumerator();
        }



    }
}
