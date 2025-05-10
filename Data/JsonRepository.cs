using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using LibraryManagementSystem.Interfaces;

namespace LibraryManagementSystem.Data
{
    public class JsonRepository<T> : IRepository<T> where T : class
    {
        private readonly string _filePath;
        private List<T> _entities;

        public JsonRepository(string fileName)
        {
            string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../"));
            string dataDirectory = Path.Combine(projectDirectory, "Data");
   
            // Create the data directory if it doesn't exist
            if (!Directory.Exists(dataDirectory))
            {
                try
                {
                    Directory.CreateDirectory(dataDirectory);
                    Console.WriteLine($"Created data directory at: {dataDirectory}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating data directory: {ex.Message}");
                    // Fall back to the execution directory if we can't create in project folder
                    dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                    Directory.CreateDirectory(dataDirectory);
                    Console.WriteLine($"Falling back to execution directory: {dataDirectory}");
                }
            }
            
            _filePath = Path.Combine(dataDirectory, fileName);
            Console.WriteLine($"Using data file: {_filePath}");
            LoadData();
        }

        private void LoadData()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    string json = File.ReadAllText(_filePath);
                    if (!string.IsNullOrEmpty(json))
                    {
                        _entities = JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }) ?? new List<T>();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading data from {_filePath}: {ex.Message}");
                    // Create a new list if loading fails
                    _entities = new List<T>();
                    return;
                }
            }
            
            // Initialize with empty list if file doesn't exist
            _entities = new List<T>();
            
            // Create the file with an empty array to ensure it exists
            SaveChanges();
        }

        public List<T> GetAll()
        {
            return _entities;
        }

        public T GetById(Guid id)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("Entity does not have an Id property.");
                
            return _entities.FirstOrDefault(e => 
                (Guid)idProperty.GetValue(e) == id);
        }

        public void Add(T entity)
        {
            _entities.Add(entity);
        }

        public void Update(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("Entity does not have an Id property.");
                
            Guid id = (Guid)idProperty.GetValue(entity);
            int index = _entities.FindIndex(e => 
                (Guid)idProperty.GetValue(e) == id);
                
            if (index >= 0)
                _entities[index] = entity;
        }

        public void Delete(Guid id)
        {
            var entity = GetById(id);
            if (entity != null)
                _entities.Remove(entity);
        }

        public void SaveChanges()
        {
            try
            {
                string json = JsonSerializer.Serialize(_entities, new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data to {_filePath}: {ex.Message}");
                throw; // Re-throw to allow calling code to handle the error
            }
        }
    }
}