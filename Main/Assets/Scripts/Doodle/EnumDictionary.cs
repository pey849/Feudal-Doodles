using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Object = UnityEngine.Object;

[Serializable]
public abstract class EnumDictionary : ISerializationCallbackReceiver
{
    [SerializeField]
    private string EnumTypeName;

    [SerializeField]
    private string ObjectTypeName;

    [SerializeField]
    private Entry[] Entries;

    [Serializable]
    public struct Entry
    {
        public int Key;
        public Object Val;
    }

    protected EnumDictionary( Type enumType, Type objectType )
    {
        EnumTypeName = enumType.AssemblyQualifiedName;
        ObjectTypeName = objectType.AssemblyQualifiedName;
    }

    public Object GetObject( Enum key )
    {
        var eVal = Convert.ToInt32( key );
        return Entries.First( x => x.Key == eVal ).Val;
    }

    private Array GetEnumValues()
    {
        var enumType = Type.GetType( EnumTypeName );
        return Enum.GetValues( enumType );
    }

    private void CreateEntries()
    {
        var enums = GetEnumValues();

        // 
        Entries = new Entry[enums.Length];
        for( int i = 0; i < Entries.Length; i++ )
        {
            var eVal = (int) enums.GetValue( i );
            Entries[i] = new Entry { Key = eVal, Val = null };
        }
    }

    private void AdjustEntries()
    {
        var enums = GetEnumValues();

        // 
        if( Entries.Length != enums.Length )
        {
            var entr = new List<Entry>();
            foreach( int eVal in enums )
            {
                var idx = Array.FindIndex( Entries, x => x.Key == eVal );
                if( idx >= 0 ) entr.Add( Entries[idx] );
                else entr.Add( new Entry { Key = eVal, Val = null } );
            }

            Entries = entr.ToArray();
        }
    }

    public void OnBeforeSerialize()
    {
        if( Entries == null ) CreateEntries();
        else AdjustEntries();
    }

    public void OnAfterDeserialize()
    {

        // 
    }
}
