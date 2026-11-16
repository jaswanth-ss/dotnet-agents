using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
namespace agenticCalculator
{
    public class CalculatorPlugin
    {
        [KernelFunction("Add")]
        [Description("To add two numbers")]
        public double Add(double a, double b)
        {
            Console.WriteLine($"[Tool] Add({a}, {b})");
            return a+b;
        }

        [KernelFunction("Subtract")]
        [Description("To subtract two numbers")]
        public double Subtract(double a, double b)
        {
            Console.WriteLine($"[Tool] Subtract({a}, {b})");
            return a - b;
        }

        [KernelFunction("Multiply")]
        [Description("To multiply two numbers")]
        public double Multiply(double a, double b)
        {
            Console.WriteLine($"[Tool] Multiply({a}, {b})");
            return a * b;
        }

        [KernelFunction("Divide")]
        [Description("To divide two numbers")]
        public double Divide(double a, double b)
        {
            Console.WriteLine($"[Tool] Divide({a}, {b})");
            return a/b;
        }

        [KernelFunction("Percentage")]
        [Description("To find the percentage of given number")]
        public double Percentage(double a, double b)
        {
            Console.WriteLine($"[Tool] Percentage({a}, {b})");
            return a * b / 100;
        }
    }
}