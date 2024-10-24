﻿using StringMessagesApiContracts;

namespace ReCounterContracts;

public interface IProgressDataMessenger : IMessenger
{
    Task SendProgressData(ProgressData progressData, CancellationToken cancellationToken);
}