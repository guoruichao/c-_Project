using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Data;
using libDB;

namespace libDB
{
    internal class DynamicBuilder<T> where T : IModel, new()
    {
        private static readonly MethodInfo getValueMethod = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo isDBNullMethod = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });
        private delegate T Load(IDataRecord dataRecord, T t_Model);
        private delegate object GetValue(T t_Model);
        private delegate T SetValue(T t_Model, object value);
        private Load handler;
        private GetValue getValue;
        private SetValue setValue;
        private DynamicBuilder() { }

        public T BuildModel(IDataRecord dataRecord, T t_Model)
        {
            return handler(dataRecord, t_Model);
        }


        public object BuildValue(T t_Model)
        {
            return getValue(t_Model);
        }

        public T SetModelValue(T t_Model, object value)
        {
            return setValue(t_Model, value);
        }

        internal static DynamicBuilder<T> CreateValueBuilder(T t_Model, FieldAttribute fieldAttribute)
        {
            DynamicBuilder<T> dynamicBuilder = new DynamicBuilder<T>();
            DynamicMethod method = new DynamicMethod("DynamicValueCreate", typeof(object), new Type[] { typeof(T) }, typeof(object), true);
            //中间语言容器,用于创建动态类型.
            ILGenerator generator = method.GetILGenerator();
            PropertyInfo propertyInfo = AttributeCache<T>.GetModelPropertyInfoExact(fieldAttribute.ColumnsName);
            generator.Emit(OpCodes.Ldarg_0);
            generator.EmitCall(OpCodes.Call, propertyInfo.GetGetMethod(), null);
            generator.Emit(OpCodes.Box, propertyInfo.PropertyType);
            generator.Emit(OpCodes.Ret);
            dynamicBuilder.getValue = (GetValue)method.CreateDelegate(typeof(GetValue));
            return dynamicBuilder;
        }

        internal static DynamicBuilder<T> CreateValueSetter(FieldAttribute fieldAttribute)
        {
            DynamicBuilder<T> dynamicBuilder = new DynamicBuilder<T>();
            DynamicMethod method = new DynamicMethod("DynamicValueSet", typeof(T), new Type[] { typeof(T), typeof(object) }, typeof(T), true);
            //中间语言容器,用于创建动态类型.
            ILGenerator generator = method.GetILGenerator();
            PropertyInfo propertyInfo = AttributeCache<T>.GetModelPropertyInfoExact(fieldAttribute.ColumnsName);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            if (propertyInfo.PropertyType.IsValueType)
            {
                generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
            }
            generator.EmitCall(OpCodes.Call, propertyInfo.GetSetMethod(), null);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ret);
            dynamicBuilder.setValue = (SetValue)method.CreateDelegate(typeof(SetValue));
            return dynamicBuilder;
        }

        public static DynamicBuilder<T> CreateModelBuilder(IDataRecord dataRecord)
        {
            DynamicBuilder<T> dynamicBuilder = new DynamicBuilder<T>();
            DynamicMethod method = new DynamicMethod("DynamicModelCreate", typeof(T), new Type[] { typeof(IDataRecord), typeof(T) }, typeof(T), true);
            //中间语言容器,用于创建动态类型.
            ILGenerator generator = method.GetILGenerator();

            for (int i = 0; i < dataRecord.FieldCount; i++)
            {

                //获得该类型的所有属性.
                PropertyInfo propertyInfo = AttributeCache<T>.GetModelPropertyInfoExact(dataRecord.GetName(i));
                if (propertyInfo == null)
                    propertyInfo = AttributeCache<T>.GetModelPropertyInfo(dataRecord.GetName(i));
                //创建一个新标签.
                Label endIfLabel = generator.DefineLabel();
                if (propertyInfo != null)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                    generator.Emit(OpCodes.Brtrue, endIfLabel);

                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, getValueMethod);
                    //if (!propertyInfo.PropertyType.IsValueType)
                    generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                    generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                    generator.MarkLabel(endIfLabel);
                }
            }
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Ret);

            dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
            return dynamicBuilder;
        }

    }

}
