﻿using System;
using System.Linq;
using System.Collections.Generic;
using DaiDaKaram.Domain.Entities;
using DaiDaKaram.Domain.Infrastructure;

namespace DaiDaKaram.Domain
{
	public class CompatibilityService : ICompatibilityService
	{
		readonly IRepository<Compatibility> _compatibilityRepository;
		readonly ICarRepository _carRepository;

		public CompatibilityService (IRepository<Compatibility> compatRepo, ICarRepository carRepository)
		{
			_compatibilityRepository = compatRepo;
			_carRepository = carRepository;
		}

		public IEnumerable<string> GetMakes ()
		{
			return _carRepository.GetMakes ();
		}

		public IEnumerable<string> GetModels (string make)
		{
			return _carRepository.GetModels (make);
		}

		public IEnumerable<string> GetVariants (string make, string model)
		{
			return _carRepository.GetVariants(make, model);
		}

		public IEnumerable<string> GetBodies (string make, string model)
		{
			return _carRepository.GetBodies (make, model);
		}

		public int GetYearFrom (string make, string model)
		{
			return _carRepository.GetYearFrom (make, model);
		}

		public int GetYearTo (string make, string model)
		{
			return _carRepository.GetYearTo (make, model);
		}

		public IEnumerable<string> GetTypes (string make, string model)
		{
			return _carRepository.GetTypes (make, model);
		}
	}
}