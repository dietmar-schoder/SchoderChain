# SchoderChain

## What problem does this solve?

I wanted a generic pattern that allows me to process anything in small, consecutive steps.

But whenever any of these steps fails, I wanted
- the process to stop,
- the steps already done to be undone in the correct order, and
- to receive a Slack message telling me how far the process came and with what error it failed.

## How does this solve the problem?

I tried it with a [chain-of-responsibility pattern](https://en.wikipedia.org/wiki/Chain-of-responsibility_pattern) and it works.

The unit tests show how it is used, see: [GitHub](https://github.com/dietmar-schoder/SchoderChain/blob/master/SchoderChainUnitTests/ChainTests.cs)
