// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GCT.MedPro.Database
{
    public static class EntityExtensions
    {
        /// <summary>
        /// GUID 类型
        /// </summary>
        private static Type Type_Guid { get; } = typeof(Guid);

        /// <summary>
        /// 实体缓存
        /// </summary>
        private static Dictionary<Type, bool> EntityTypeCache { get; } = new Dictionary<Type, bool>();

        /// <summary>
        /// GUID类型实体
        /// </summary>
        private static Dictionary<Type, bool> GuidEntityTypeCache { get; } = new Dictionary<Type, bool>();

        /// <summary>
        /// 是否为数据库实体类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEntity(this Type type)
        {
            return IsEntity(type, out var keyType);
        }

        /// <summary>
        /// 使用Oracle的映射规则
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <returns></returns>
        public static ModelBuilder MedProUseOracleTableMapping(this ModelBuilder modelBuilder)
        {
            var verifyingEntityType = new Func<IMutableEntityType, bool>((e) =>
            {
                return e.ClrType.IsEntity();
            });

            return modelBuilder
                .TableMappingToDevartOracle(verifyingEntityType)
                .MapDiscriminators(verifyingEntityType);
        }

        /// <summary>
        /// 是否为数据库实体类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="keyType"></param>
        /// <returns></returns>
        public static bool IsEntity(this Type type, out Type keyType)
        {
            keyType = null;
            if (EntityTypeCache.TryGetValue(type, out var res))
            {
                return res;
            }

            var idProp = type.GetProperty("Id");
            keyType = idProp?.PropertyType;
            if (idProp == null)
            {
                return false;
            }

            res = typeof(IEntity<>).MakeGenericType(idProp.PropertyType).IsAssignableFrom(type);
            EntityTypeCache.Add(type, res);

            return res;
        }

        /// <summary>
        /// 是否为guid类型的数据库实体类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsGuidEntity(this Type type)
        {
            if (GuidEntityTypeCache.TryGetValue(type, out var res))
            {
                return res;
            }

            if (!IsEntity(type, out var keyType))
            {
                GuidEntityTypeCache.Add(type, false);
                return false;
            }

            res = keyType == Type_Guid;
            GuidEntityTypeCache.Add(type, res);
            return res;
        }
    }

    public interface IEntity<T>
    {
        //
        // 摘要:
        //     Unique identifier for this entity.
        T Id { get; set; }

        //
        // 摘要:
        //     Checks if this entity is transient (not persisted to database and it has not
        //     an Abp.Domain.Entities.IEntity`1.Id).
        //
        // 返回结果:
        //     True, if this entity is transient
    }

    public abstract class Entity<T> : IEntity<T>
    {
        public T Id { get; set; }
    }
}
