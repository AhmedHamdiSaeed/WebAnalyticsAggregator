using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Implementations;
using Application.Interfaces;
using DTOs;
using Infrastructure.Entities;
using Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Producer.Services;

namespace Tests.UnitTests
{
    public class JsonAdapterTests
    {
        private readonly IDataReader<GARecord> _gaReader;
        private readonly IDataReader<PSIRecord> _psiReader;
        public JsonAdapterTests()
        {
            _gaReader = new GADataReader(); // your concrete implementation
            _psiReader = new PSIDataReader();
        }
        [Fact]
        public async Task GAAdapter_ShouldParseJsonCorrectly()
        {
            // Act
            var result = await _gaReader.ReadAsync("mock-data/ga-data.json");

            // Assert
            Assert.Equal("/home", result[0].page);
            Assert.Equal(120, result[0].users);
        }

        [Fact]
        public async Task PSIAdapter_ShouldParseJsonCorrectly()
        {

            // Act
            var psiData = await _psiReader.ReadAsync("mock-data/psi-data.json");

            // Assert
            Assert.Equal(0.9m, psiData[0].performanceScore);
            Assert.Equal(2100, psiData[0].LCP_ms);
        }
    }
    }
