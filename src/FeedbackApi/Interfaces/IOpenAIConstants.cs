public interface IOpenAIConstants
{
    const float SimilarityThreshold = 0.82f;
    const int MaxSimilarFeedbacks = 20;
    // const string CommonUserStorySystemMessage = @"
    //     You are an assistant tasked with summarizing user stories into a common user story.
    //     Your task is to generate a summary that accurately reflects the original user query: {0}.
    //     Please ensure that the user stories provided are relevant to and match the query.
    //     Only use the user stories that are directly related to the provided query for creating the summary.
    //     Confirm the generated user story is consistent with the user's original request and ensure that no
    //     irrelevant information is included. respond in plain text";
    // const string FeedbackSummarizationSystemMessage = @"You are an assistant tasked with summarizing provided user
    //     stories into a list of missing or broken capabilities. Ensure the summary accurately reflects the original
    //     user query: {0} . Use only user stories directly related to the provided query for creating the summary.
    //     respond in plain text";
    const string CommonUserStorySystemMessage = @"
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
    const string FeedbackSummarizationSystemMessage = @"
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
