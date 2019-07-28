﻿using AHP.Service.Common;
using AHP.Service.Common.AHPCalculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Service.CalculationClasses
{
    public class FinalResultCalculator : IFinalResultCalculator
    {
        public FinalResultCalculator(IPriorityCalculator priorityCalculator,IAlternativeService alternativeService, IMatrixCreator matrixCreator, IDataPreparation dataPreparation, ICriteriaService criteriaService)
        {
            this.PriorityCalculator = priorityCalculator;
            this.MatrixCreator = matrixCreator;
            this.DataPreparation = dataPreparation;
            this.CriteriaService = criteriaService;
            this.AlternativeService = alternativeService;
        }
        IPriorityCalculator PriorityCalculator;
        IMatrixCreator MatrixCreator;
        IDataPreparation DataPreparation;
        ICriteriaService CriteriaService;
        IAlternativeService AlternativeService;

        public double[] AHPMethod(double[] CriteriaPreference, double[][] AlternativePreferences)
        {
            /// <summary>
            /// Method for calculating final decision using AHP method, with given criteria preferences and alternative preference for every criterion.
            /// </summary>
            /// <param name="CriteriaPreference">Array of criterion preferences</param>
            /// <param name="AlternativePreferences">Jagged array of alternative preferences for every criterion</param>
            /// <returns>
            /// Array of priorities for every alternative.
            /// </returns>

            //int[] Kriterij = new int[(m * m - m) / 2];
            //int[] Kriterij = new int[] { 1, 3, 5 };

            int numberOfCriterions = CriteriaPreference.Length;
            int numberOfAlternatives = AlternativePreferences.Length;

            double[] CriteriaPriority = new double[numberOfCriterions];
            CriteriaPriority = DataPreparation.NormalizeVector(PriorityCalculator.CalculatePriority(MatrixCreator.CreateMatrix(CriteriaPreference, numberOfCriterions)));

            // Matrix containing final weights
            // w_{i,j} = priority of i-th alternative considering j-th criterion
            double[,] W = new double[numberOfAlternatives, numberOfCriterions];

            for (int i = 0; i < numberOfCriterions; i++)
            {
                // Array of Alternative preferences considering i-th criterion
                double[] currentAlternatives = AlternativePreferences[i];

                // Array of priorities considering i-th criterion
                double[] AlternativePriority =DataPreparation.NormalizeVector( PriorityCalculator.CalculatePriority(MatrixCreator.CreateMatrix(currentAlternatives, numberOfAlternatives)));

                for (int j = 0; j < numberOfAlternatives; j++)
                {
                    // Setting i-th column of matrix W to priorities of all alternatives
                    W[j, i] = AlternativePriority[j];
                }
            }

            double[] FinalDecision = new double[numberOfAlternatives];

            for (int i = 0; i < numberOfAlternatives; i++)
            {
                for (int j = 0; j < numberOfCriterions; j++)
                {
                    FinalDecision[i] += W[i, j] * CriteriaPriority[j];
                }
            }
            return DataPreparation.NormalizeVector(FinalDecision);
        }

        public async Task<bool> Calculate(int id)
        {
            var criterias = await CriteriaService.GetCriteriasByProjectIdWithCRaAR(id);
            var array2d = DataPreparation.Get2dArray(criterias);
            var criteriaRanks = DataPreparation.GetCriteriaRanks(criterias);

            var result = AHPMethod(criteriaRanks, array2d);

            var alternatives =  await AlternativeService.GetAlternativesByProjectId(id,1,20);
            for(int i = 0; i < alternatives.Count(); i++)
            {
                alternatives[i].FinalPriority = result[i];
            }
            //TODO : save changes to db

            return true;
        }


    }
}
