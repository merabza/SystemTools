using MessagingAbstractions;

namespace ReCounterDom;

public record CancelCurrentProcessCommandRequest : ICommand<bool>;