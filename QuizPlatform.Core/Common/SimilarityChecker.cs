using FuzzySharp;

namespace QuizPlatform.Core.Common
{
    public class SimilarityChecker
    {
        private readonly int _threshold;

        public SimilarityChecker(int threshold = 80)
        {
            _threshold = threshold;
        }

        public (bool IsSimilar, int SimilarityScore, string SimilarQuestion)
            CheckSimilarity(string newQuestionText, IEnumerable<string> existingQuestions)
        {
            if (string.IsNullOrWhiteSpace(newQuestionText) || existingQuestions == null)
                return (false, 0, string.Empty);

            string normalizedNew = newQuestionText.Trim().ToLower();

            int maxScore = 0;
            string closestQuestion = null;

            foreach (var question in existingQuestions)
            {
                string normalizedExisting = question?.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(normalizedExisting)) continue;

                int score = Fuzz.Ratio(normalizedNew, normalizedExisting);

                if (score > maxScore)
                {
                    maxScore = score;
                    closestQuestion = question;
                }

                if (score >= _threshold)
                {
                    return (true, score, question);
                }
            }

            return (false, maxScore, closestQuestion ?? string.Empty);
        }
    }
}
