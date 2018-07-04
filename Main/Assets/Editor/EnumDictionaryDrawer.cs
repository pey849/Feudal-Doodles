using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer( typeof( EnumDictionary ), true )]
public class EnumDictionaryDrawer : PropertyDrawer
{
    public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
    {
        var y = 0F;
        var _entries = property.FindPropertyRelative( "Entries" );
        for( int i = 0; i < _entries.arraySize; i++ )
        {
            var el = _entries.GetArrayElementAtIndex( i );
            y += EditorGUIUtility.singleLineHeight * 1.2F;
        }

        return y;
    }

    public override void OnGUI( Rect frame, SerializedProperty property, GUIContent gui_label )
    {
        var obj = property.serializedObject;
        var _entries = property.FindPropertyRelative( "Entries" );
        var _enumType = property.FindPropertyRelative( "EnumTypeName" );

        EditorGUI.BeginProperty( frame, gui_label, property );

        var type = System.Type.GetType( _enumType.stringValue );
        var enumNames = System.Enum.GetNames( type );

        var rect = new Rect( frame.x, frame.y, frame.width, EditorGUIUtility.singleLineHeight * 1.25F );

        for( int i = 0; i < _entries.arraySize; i++ )
        {
            var el = _entries.GetArrayElementAtIndex( i );
            var key = el.FindPropertyRelative( "Key" );
            var val = el.FindPropertyRelative( "Val" );

            var enumName = ObjectNames.NicifyVariableName( enumNames[key.intValue] );

            EditorGUI.BeginProperty( rect, gui_label, el );

            var box = EditorGUI.PrefixLabel( rect, new GUIContent( enumName ) );
            box.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.ObjectField( box, val, GUIContent.none );

            // position.y += EditorGUI.GetPropertyHeight( el );
            rect.y += EditorGUIUtility.singleLineHeight * 1.2F;

            EditorGUI.EndProperty();
        }

        EditorGUI.EndProperty();
    }
}