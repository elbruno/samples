﻿// <Snippet1>
using System;
using Microsoft.ML.Models;
using Microsoft.ML.Runtime;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
// </Snippet1>

namespace SentimentAnalysis
{
    class Program
    {
        // <Snippet2>
        const string _dataPath = @"..\..\data\sentiment labelled sentences\imdb_labelled.txt";
        const string _testDataPath = @"..\..\data\sentiment labelled sentences\yelp_labelled.txt";
        // </Snippet2>

        static void Main(string[] args)
        {
            // <Snippet3>
            var model = TrainAndPredict();
            // </Snippet3>
            // <Snippet17>
            Evaluate(model);
            // </Snippet17>
        }

        // <Snippet4>
        public static PredictionModel<SentimentData, SentimentPrediction> TrainAndPredict()
        // </Snippet4>
        {
            // LearningPipeline allows us to add steps in order to keep everything together 
            // during the learning process.  
            // <Snippet5>
            var pipeline = new LearningPipeline();
            // </Snippet5>

            // The TextLoader loads a dataset with comments and corresponding postive or negative sentiment. 
            // When you create a loader you specify the schema by passing a class to the loader containing
            // all the column names and their types. This will be used to create the model, and train it. 
            // <Snippet6>
            pipeline.Add(new TextLoader<SentimentData>(_dataPath, useHeader: false, separator: "tab"));
            // </Snippet6>

            // TextFeaturizer is a transform that will be used to featurize an input column. 
            // This is used to format and clean the data.
            // <Snippet7>
            pipeline.Add(new TextFeaturizer("Features", "SentimentText"));
            //</Snippet7>

            //add a FastTreeBinaryClassifier, the decision tree learner for this project, and 
            //three hyperparameters to be used for tuning decision tree performance 
            // <Snippet8>
            pipeline.Add(new FastTreeBinaryClassifier() { NumLeaves = 5, NumTrees = 5, MinDocumentsInLeafs = 2 });
            // </Snippet8>

            // We train our pipeline based on the dataset that has been loaded, transformed 
            // <Snippet9>
            PredictionModel<SentimentData, SentimentPrediction> model = 
                pipeline.Train<SentimentData, SentimentPrediction>();
            // </Snippet9>

            //add some comments to test the trained model's predictions
            // <Snippet10>
            IEnumerable<SentimentData> sentiments = new[]
             {
                new SentimentData
                {
                    SentimentText = "Contoso's 11 is a wonderful experience",
                    Sentiment = 0
                },
                new SentimentData
                {
                    SentimentText = "The acting in this movie is very bad",
                    Sentiment = 0
                },
                new SentimentData
                {
                    SentimentText = "Joe versus the Volcano Coffee Company is a great film.",
                    Sentiment = 0
                }
            };
            // </Snippet10>

            // Now that we have a model, use that to predict the positive 
            // or negative sentiment of the comment data.
            // <Snippet11>
            IEnumerable<SentimentPrediction> predictions = model.Predict(sentiments);
            // </Snippet11>

            // <Snippet12>
            Console.WriteLine();
            Console.WriteLine("Sentiment Predictions");
            Console.WriteLine("---------------------");
            // </Snippet12>

            // Build pairs of (sentiment, prediction)
            // <Snippet13>
            var sentimentsAndPredictions = sentiments.Zip(predictions, (sentiment, prediction) => (sentiment, prediction));
            // </Snippet13>

            // <Snippet14>
            foreach (var item in sentimentsAndPredictions)
            {
                Console.WriteLine($"Sentiment: {item.Item1.SentimentText} | Prediction: {(item.Item2.Sentiment ? "Positive" : "Negative")}");
            }
            Console.WriteLine();
            // </Snippet14>

            //return the model we trained to use for evaluation
            // <Snippet15>
            return model;
            // </Snippet15>
        }
        // <Snippet16>
        public static void Evaluate(PredictionModel<SentimentData, SentimentPrediction> model)
        // </Snippet16>
        {
            // Evaluate
            // <Snippet18>
            var testData = new TextLoader<SentimentData>(_testDataPath, useHeader: false, separator: "tab");
            // </Snippet18>

            // BinaryClassificationEvaluator computes the quality metrics for the PredictionModel
            //using the specified data set.
            // <Snippet19>
            var evaluator = new BinaryClassificationEvaluator();
            // </Snippet19>

            // BinaryClassificationMetrics contains the overall metrics computed by binary classification evaluators
            // <Snippet20>
            BinaryClassificationMetrics metrics = evaluator.Evaluate(model, testData);
            // </Snippet20>

            // The Accuracy metric gets the accuracy of a classifier which is the proportion 
            //of correct predictions in the test set.

            // The Auc metric gets the area under the ROC curve.
            // The area under the ROC curve is equal to the probability that the classifier ranks
            // a randomly chosen positive instance higher than a randomly chosen negative one
            // (assuming 'positive' ranks higher than 'negative').

            // The F1Score metric gets the classifier's F1 score.
            // The F1 score is the harmonic mean of precision and recall:
            //  2 * precision * recall / (precision + recall).


            // <Snippet21>
            Console.WriteLine();
            Console.WriteLine("PredictionModel quality metrics evaluation");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc: {metrics.Auc:P2}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
            // </Snippet21>
        }
    }
}
