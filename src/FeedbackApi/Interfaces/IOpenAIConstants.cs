public interface IOpenAIConstants
{
    const float SimilarityThreshold = 0.82f;
    const int MaxSimilarFeedbacks = 20;

    const string CombinedSummaryFile = "combined_service_summary.json";
    const string ServiceMappingFile = "service_mapping.json";

    const string UserStoryClassificationSystemMessage = @"Classify the following user story into a single category.
    Return only the classification label and short reasoning for the class, the classification needs to be generic but not too inclusive, so 'feature enhancement'
    is not a good classification. the response must match this json element:
    {
    ""classification"": ""<the classification label>"",
    ""reasoning"": ""<a short reasoning for the class>""
    }
    User Story:";

    const string CreateClusterByLLMSystemMessage = @"group the following user feedback items into clusters based on common themes.
    A list of existing classification can be added and can be used to classify the feedback items,
    in case new classification is required or reword existing classification, you can do that.
    Return your response as a JSON array. Each element in the array should be an object with the following keys:
    {
    ""CommonElement"": ""<the common theme for the group.>"",
    ""FeedbackIds"": ""<: an array of the IDs (as strings) of the feedback items that belong to that group.>""
    }
    Make sure the common element is clear and concise. Feedback items:";


    const string CreateClusterSystemMessage = @"Generate a JSON response with the following structure:
    {
    ""CommonElement"": ""<A concise phrase describing the common theme>"",
    ""Summary"": ""<A detailed explanation summarizing the feedback>""
    }
    Make sure the common element is clear and concise, and the summary provides a comprehensive explanation.
    You base your summary only on the provided user stories by the user.";
    const string Feedback2UserStory =
        @"You are an AI assistant. You generate clear generic user stories in text only with the following format:
        'As a [persona], I want to [do something], so that I can [achieve something].'
        The output should always follow this format without additional styling or formatting.
        Do not include specific customers/partner names as part of the output.";
    const string CommonUserStorySystemMessage =
        @"
        You are an assistant tasked with 2 actions, based on a list of feedbacks you get from the user.
        1: summarizing feedback from all customers into a main theme.
        2: per customer collect all feedbacks and summarize them. for the customer information (tpid and name) they are provided
        in the feedbackrecord list.
        Generate a JSON response with this exact following structure:

        {
          'summary_detail': {
            'main_points': [
              {
                'title': '<A concise title summarizing the key point from all customers>',
                'description': [
                  '<Each relevant description or feedback sub-point>'
                ]
              },
              ...
            ]
          },
          'customers': [
            {
              'name': '<CustomerName>',
              'tpid': '<CustomerTpid>',
              'feedback_title': '<highlights of all of this customer feedbacks>',
              'summary_detail': {
                'main_points': [
                  {
                    'title': '<A concise title summarizing the customer-specific key point>',
                    'description': [
                      '<Relevant feedback points specific to this customer>'
                    ]
                  }
                ]
              }
            },
            ...
          ]
        }

        Only generate the summary based on the feedback provided. Ensure the 'summary_detail' section gives a high-level overview of the feedback, while each customer in the 'customers' list has a customer-specific summary.
        Do not include the feedback records directly; focus only on the summary of points. Make sure the JSON is valid and well-formatted for consumption.
";
    const string FeedbackSummarizationSystemMessage =
        @"
        You are an assistant tasked with summarizing user feedback.
        Your task is to generate a response that summarizes the feedback.
        The structure of the response is in json with this structure:

        {
          'title': '<A title summarizing the feedback>',
          'summary': {
            'main_points': [
              {
                'title': '<The main point title>',
                'description': [
                  '<Each sub-point description>'
                ]
              },
              ...
            ]
          }
        }

        Ensure the 'title' provides a high-level summary of the feedback, and each 'mainPoints' section includes more detailed points.
        Each 'description' should be a concise list of relevant feedback sub-points. Make sure to validate on the query provided by the user.
        Make sure the JSON is valid and well-formatted for direct consumption in an application.
";
}
