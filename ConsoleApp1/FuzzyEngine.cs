using System;
using System.Collections.Generic;

public class FuzzyEngine
{
    /// <summary>
    /// Retorna o índice da pergunta com maior variância simples.
    /// Ideal para a primeira pergunta antes do usuário responder qualquer coisa.
    /// </summary>
    public static int ObterMelhorPerguntaPorVariancia(double[,] matrix, string[] perguntas, HashSet<int> perguntasFeitas = null, double[] probCandidatos = null)
    {
        Console.WriteLine($"\n ---escolhendo pergunta por variância--- \n");

        int totalPerguntas = matrix.GetLength(0);
        int totalCandidatos = matrix.GetLength(1);

        int melhorIndice = -1;
        double maiorVariancia = -1.0;

        for (int q = 0; q < totalPerguntas; q++)
        {
            if (perguntasFeitas != null && perguntasFeitas.Contains(q))
                continue;

            double soma = 0.0;
            for (int c = 0; c < totalCandidatos; c++)
            {
                soma += matrix[q, c];
            }
            double media = soma / totalCandidatos;

            double somaQuad = 0.0;
            for (int c = 0; c < totalCandidatos; c++)
            {
                double diff = matrix[q, c] - media;
                somaQuad += (diff * diff) * (probCandidatos[c] * 10);
            }
            double variancia = somaQuad / totalCandidatos;

            Console.WriteLine($"{perguntas[q]}: {variancia}");

            if (variancia > maiorVariancia)
            {
                maiorVariancia = variancia;
                melhorIndice = q;
            }
        }

        return melhorIndice;
    }

    /// <summary>
    /// Retorna o índice da pergunta com maior Ganho de Informação / Entropia,
    /// considerando a probabilidade atual de cada candidato ser a resposta final.
    /// </summary>
    public static int ObterMelhorPerguntaDinamica(double[,] matrix, string[] perguntas, double[] probCandidatos, HashSet<int> perguntasFeitas = null)
    {
        Console.WriteLine($"\n ---escolhendo pergunta inicial--- \n");

        int totalPerguntas = matrix.GetLength(0);
        int totalCandidatos = matrix.GetLength(1);

        int melhorIndice = -1;
        double maiorPontuacao = -1.0;

        for (int q = 0; q < totalPerguntas; q++)
        {
            if (perguntasFeitas != null && perguntasFeitas.Contains(q))
                continue;

            // 1. Probabilidade esperada de resposta "sim" ponderada pelos candidatos vivos
            double pSim = 0.0;
            for (int c = 0; c < totalCandidatos; c++)
            {
                pSim += matrix[q, c] * probCandidatos[c];
            }
            double pNao = 1.0 - pSim;

            // 2. Entropia do split (máxima perto de 0.5, dividindo o espaço de buscas ao meio)
            double entropia = 0.0;
            if (pSim > 0 && pNao > 0)
            {
                entropia = -(pSim * Math.Log(pSim, 2) + pNao * Math.Log(pNao, 2));
            }

            // 3. Variância ponderada pela relevância dos candidatos atuais
            double varianciaPonderada = 0.0;
            for (int c = 0; c < totalCandidatos; c++)
            {
                double diff = matrix[q, c] - pSim;
                varianciaPonderada += probCandidatos[c] * (diff * diff);
            }

            // Score: combina a capacidade de segregar com o equilíbrio da pergunta
            double score = varianciaPonderada * entropia;

            Console.WriteLine($"{perguntas[q]}: {score}");

            if (score > maiorPontuacao)
            {
                maiorPontuacao = score;
                melhorIndice = q;
            }
        }

        return melhorIndice;
    }

    /// <summary>
    /// Atualiza as probabilidades dos candidatos com base na resposta fuzzy dada pelo usuário (0.0 a 1.0).
    /// </summary>
    public static void AtualizarProbabilidades(double[,] matrix, int perguntaIndex, double respostaUsuario, double[] probCandidatos)
    {
        int totalCandidatos = matrix.GetLength(1);
        double somaNovasProbs = 0.0;

        for (int c = 0; c < totalCandidatos; c++)
        {
            // Complemento da distância absoluta (similaridade fuzzy)
            double similaridade = 1.0 - Math.Abs(matrix[perguntaIndex, c] - respostaUsuario);

            // Atualização Bayesiana
            probCandidatos[c] *= similaridade;
            somaNovasProbs += probCandidatos[c];
        }

        // Normalização (garante que a soma das probabilidades seja 1.0)
        if (somaNovasProbs > 0)
        {
            for (int c = 0; c < totalCandidatos; c++)
            {
                probCandidatos[c] /= somaNovasProbs;
            }
        }
    }
}