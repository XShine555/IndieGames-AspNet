namespace Infrastructure.MassTransit.Helpers
{
    internal static class EndpointHelper
    {
        internal static Uri BuildExecuteActivityUri(string endpointName) => new($"queue:{BuildExecuteActivityEndpointName(endpointName) }");

        internal static Uri BuildCompensateActivityUri(string endpointName) => new($"queue:{BuildCompensateActivityEndpointName(endpointName) }");

        internal static Uri BuildConsumerUri(string endpointName) => new($"queue:{BuildConsumerEndpointName(endpointName) }");

        internal static string BuildExecuteActivityEndpointName(string endpointName) => $"{endpointName}_execute";

        internal static string BuildCompensateActivityEndpointName(string endpointName) => $"{endpointName}_compensate";

        internal static string BuildConsumerEndpointName(string endpointName) => endpointName;
    }
}