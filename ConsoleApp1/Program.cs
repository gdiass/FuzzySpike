using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] perguntas, candidatos;
            double[,] matrizFuzzy;
            PreencherTabelas(out perguntas, out candidatos, out matrizFuzzy);

            int totalCandidatos = candidatos.Length;
            double[] probCandidatos = new double[totalCandidatos];

            // Distribuição inicial uniforme (1.0 / N)
            for (int c = 0; c < totalCandidatos; c++) probCandidatos[c] = 1.0 / totalCandidatos;

            var perguntasFeitas = new HashSet<int>();

            // Primeira pergunta via algoritmo dinâmico
            int proximaPergunta = FuzzyEngine.ObterMelhorPerguntaDinamica(matrizFuzzy, perguntas, probCandidatos, perguntasFeitas);

            Console.WriteLine($"\n Melhor Pergunta Selecionada: {perguntas[proximaPergunta]}\n");

            // Input do usuário
            string inputUsuario = Console.ReadLine();
            if (!double.TryParse(inputUsuario, out double respostaUsuario))
            {
                throw new ArgumentException("O valor digitado não é um número válido.");
            }

            //respostaUsuario = 0.75;
            Console.WriteLine($"#Input do Usuário: {respostaUsuario}");
            perguntasFeitas.Add(proximaPergunta);

            // Atualiza a distribuição de probabilidades dos candidatos
            FuzzyEngine.AtualizarProbabilidades(matrizFuzzy, proximaPergunta, respostaUsuario, probCandidatos);

            // Exibe probabilidades atualizadas
            for (int c = 0; c < totalCandidatos; c++)
            {
                Console.WriteLine($"{candidatos[c]}: {probCandidatos[c]:P1}");
            }

            for (int i = 0; i < 2; i++) // Exemplo: fazer 2 perguntas adicionais
            {
                NextQuestion(perguntas, candidatos, matrizFuzzy, totalCandidatos, probCandidatos, perguntasFeitas, out proximaPergunta, out inputUsuario, out respostaUsuario);
            }

            var resultado = candidatos.OrderByDescending(x => probCandidatos[Array.IndexOf(candidatos, x)]).First();
            Console.WriteLine($"\n !!!!!!!! O resultado é !!!!!!!!!!!!!!");
            Console.WriteLine($"\n {resultado}");

            inputUsuario = Console.ReadLine();
        }

        private static void PreencherTabelas(out string[] perguntas, out string[] candidatos, out double[,] matrizFuzzy)
        {
            perguntas = new[] {
                "É brinquedo de 'menino'?",
                "É de brincar em grupo?",
                "É para brincar ao ar livre?",
                "Pode molhar?"
            };
            candidatos = new [] { "Baralho", "Bola", "Boneca", "Pelúcia", "Quebra-cabeça" };

            // Matriz [Linhas = Perguntas, Colunas = Candidatos]
            matrizFuzzy = new double[,] {
                { 0.50, 1.00, 0.00, 0.25, 0.50 }, // Q0
                { 1.00, 0.75, 0.50, 0.50, 0.25 }, // Q1
                { 0.25, 1.00, 0.50, 0.00, 0.00 }, // Q2
                { 0.00, 1.00, 0.75, 0.00, 0.00 }  // Q3
            };
        }

        private static void PreencherTabelas2(out string[] perguntas, out string[] candidatos, out double[,] matrizFuzzy)
        {
            perguntas = new[] {
                "É brinquedo de 'menino'?",
                "É de brincar em grupo?",
                "É para brincar ao ar livre?",
                "Pode molhar?"
            };
            candidatos = new[] { "Baralho", "Bola", "Boneca", "Pelúcia", "Quebra-cabeça" };

            // Matriz [Linhas = Perguntas, Colunas = Candidatos]
            matrizFuzzy = new double[,] {
                { 0.50, 1.00, 0.00, 0.25, 0.50 }, // Q0
                { 1.00, 0.75, 0.50, 0.50, 0.25 }, // Q1
                { 0.25, 1.00, 0.50, 0.00, 0.00 }, // Q2
                { 0.00, 1.00, 0.75, 0.00, 0.00 }  // Q3
            };
        }

        private static void NextQuestion(string[] perguntas, string[] candidatos, double[,] matrizFuzzy, int totalCandidatos, double[] probCandidatos, HashSet<int> perguntasFeitas, out int proximaPergunta, out string inputUsuario, out double respostaUsuario)
        {
            proximaPergunta = FuzzyEngine.ObterMelhorPerguntaPorVariancia(matrizFuzzy, perguntas, perguntasFeitas, probCandidatos);
            Console.WriteLine($"\nPróxima melhor pergunta selecionada: {perguntas[proximaPergunta]} \n");


            // Input do usuário
            inputUsuario = Console.ReadLine();
            if (!double.TryParse(inputUsuario, out respostaUsuario))
            {
                throw new ArgumentException("O valor digitado não é um número válido.");
            }

            //respostaUsuario = 0.25;
            Console.WriteLine($"#Input do Usuário: {respostaUsuario}");

            perguntasFeitas.Add(proximaPergunta);

            // Atualiza a distribuição de probabilidades dos candidatos
            FuzzyEngine.AtualizarProbabilidades(matrizFuzzy, proximaPergunta, respostaUsuario, probCandidatos);

            // Exibe probabilidades atualizadas
            for (int c = 0; c < totalCandidatos; c++)
            {
                Console.WriteLine($"{candidatos[c]}: {probCandidatos[c]:P1}");
            }
        }
    }
}
