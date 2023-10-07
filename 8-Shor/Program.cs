// SOURCE
// https://github.com/microsoft/Quantum
// https://github.com/microsoft/Quantum/tree/main/samples/algorithms/integer-factorization

using System;
using Microsoft.Quantum.Simulation.Simulators;
using Microsoft.Quantum.Simulation.Core;
using Shor;

namespace Microsoft.Quantum.Samples.IntegerFactorization
{
    /// <summary>
    /// This is a Console program that runs Shor's algorithm 
    /// on a Quantum Simulator.
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {
            var number = System.Convert.ToInt32(args[0]);

            // Repeat Shor's algorithm multiple times as the algorithm is 
            // probabilistic and there are several ways that it can fail.
            for (int i = 0; i < 10; ++i)
            {
                try
                {
                    // Make sure to use simulator within using block. 
                    // This ensures that all resources used by QuantumSimulator
                    // are properly released if the algorithm fails and throws an exception.
                    using var sim = new QuantumSimulator();

                    // Report the number being factored to the standard output
                    Console.WriteLine($"==========================================");
                    Console.WriteLine($"Factoring {number}");

                    // Compute the factors
                    (long factor1, long factor2) = FactorSemiprimeInteger.Run(sim, number).Result;

                    Console.WriteLine($"Factors are {factor1} and {factor2}");

                    // Stop once the factorization has been found
                    break;
                }
                // Shor's algorithm is a probabilistic algorithm and can fail with certain 
                // probability in several ways. For more details see Shor.qs.
                // If the run of Shor's algorithm fails it throws ExecutionFailException.
                // However, due to the use of System.Task in .Run method,
                // the exception of interest is getting wrapped into AggregateException.
                catch (AggregateException e )
                {
                    // Report the failure of the algorithm to standard output 
                    Console.WriteLine($"This run of Shor's algorithm failed:");

                    // Unwrap AggregateException to get the message from Q# fail statement.
                    // Go through all inner exceptions.
                    foreach (Exception eInner in e.InnerExceptions)
                    {
                        // If the exception of type ExecutionFailException
                        if (eInner is ExecutionFailException failException)
                        {
                            // Print the message it contains
                            Console.WriteLine($"   {failException.Message}");
                        }
                    }
                }
            }

            return 0;
        }
    }
}