using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq.Expressions;

namespace ConsoleApplication1
{
    // http://panicoenlaxbox.blogspot.com.es/2015/11/crear-indices-en-ef-con-fluent-api.html
    static class ModelBuilderExtensions
    {
        private static void CreateIndex(
            PrimitivePropertyConfiguration property,
            bool isUnique = false,
            string name = null,
            int position = 1)
        {
            var indexAttribute = new IndexAttribute();
            if (!string.IsNullOrWhiteSpace(name))
            {
                indexAttribute = new IndexAttribute(
                    name,
                    position <= 0 ? 1 : position);
            }
            indexAttribute.IsUnique = isUnique;
            property.HasColumnAnnotation(
                IndexAnnotation.AnnotationName,
                new IndexAnnotation(indexAttribute));
        }

        public static void Index<T, T2>(
            this EntityTypeConfiguration<T> entity,
            Expression<Func<T, T2>> propertyExpression,
            bool isUnique = false,
            string name = null,
            int position = 1) where T : class where T2 : struct
        {
            var property = entity.Property(propertyExpression);
            CreateIndex(property, isUnique, name, position);
        }

        public static void Index<T, T2>(
            this EntityTypeConfiguration<T> entity,
            Expression<Func<T, T2?>> propertyExpression,
            bool isUnique = false,
            string name = null,
            int position = 1) where T : class where T2 : struct
        {
            var property = entity.Property(propertyExpression);
            CreateIndex(property, isUnique, name, position);
        }

        public static void Index<T>(
            this EntityTypeConfiguration<T> entity,
            Expression<Func<T, string>> propertyExpression,
            bool isUnique = false,
            string name = null,
            int position = 1) where T : class
        {
            var property = entity.Property(propertyExpression);
            CreateIndex(property, isUnique, name, position);
        }
    }
}