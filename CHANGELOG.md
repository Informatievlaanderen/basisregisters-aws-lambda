## [3.0.3](https://github.com/informatievlaanderen/basisregisters-aws-lambda/compare/v3.0.2...v3.0.3) (2023-09-04)

## [3.0.2](https://github.com/informatievlaanderen/basisregisters-aws-lambda/compare/v3.0.1...v3.0.2) (2023-09-04)


### Bug Fixes

* convert JObject to Sqs Event ([d66f154](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/d66f154dd01ea2cd2df92fc6a3026a9cf86b4bd8))

## [3.0.1](https://github.com/informatievlaanderen/basisregisters-aws-lambda/compare/v3.0.0...v3.0.1) (2023-09-01)


### Bug Fixes

* handle JObject ([e66c622](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/e66c6224007aad79941a1bdd57c55f8c0a4a9eac))

# [3.0.0](https://github.com/informatievlaanderen/basisregisters-aws-lambda/compare/v2.0.1...v3.0.0) (2023-09-01)


### Features

* change handler to handle more than sqs events + ping event ([e6485ac](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/e6485acb51268d03f32325d9c389220a24c0d764))


### BREAKING CHANGES

* Handler changed accepting parameters from SQSEvent to object

## [2.0.1](https://github.com/informatievlaanderen/basisregisters-aws-lambda/compare/v2.0.0...v2.0.1) (2023-03-22)


### Bug Fixes

* style to trigger build ([4ba2d5b](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/4ba2d5b7e051695ec2a9996b228a5340034c16ac))

# [2.0.0](https://github.com/informatievlaanderen/basisregisters-aws-lambda/compare/v1.4.1...v2.0.0) (2023-03-22)


### Code Refactoring

* use JsonSerializationSettings throughout library ([5cc42a0](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/5cc42a092b182366d85c864a4d85a2574e5b6713))


### BREAKING CHANGES

* use JsonSerializationSettings in all static methods and in FunctionBase

## [1.4.1](https://github.com/informatievlaanderen/basisregisters-aws-lambda/compare/v1.4.0...v1.4.1) (2023-03-21)


### Bug Fixes

* make SQS message deserializing customizable ([e04036f](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/e04036fc3c05a725564b1b63de24b12d57328982))

# [1.4.0](https://github.com/informatievlaanderen/basisregisters-aws-lambda/compare/v1.3.1...v1.4.0) (2023-03-13)


### Features

* add option to support graceful shutdown before termination ([4574c54](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/4574c542bfd9e690ded4ef9a19bc6230367c7c6d))

## [1.3.1](https://github.com/informatievlaanderen/basisregisters-aws-lambda/compare/v1.3.0...v1.3.1) (2022-12-09)


### Bug Fixes

* style to trigger build ([77b45ad](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/77b45ad6c91b3e4637b653cc804ba1156b25576b))

# [1.3.0](https://github.com/informatievlaanderen/basisregisters-aws-lambda/compare/v1.2.4...v1.3.0) (2022-12-09)


### Features

* expose ServiceProvider to the FunctionBase implementation ([2314521](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/231452120352555f44a204970c093dd5c2d144ab))

## [1.2.4](https://github.com/informatievlaanderen/basisregisters-aws-lambda/compare/v1.2.3...v1.2.4) (2022-09-23)

## [1.2.3](https://github.com/informatievlaanderen/basisregisters-aws-lambda/compare/v1.2.2...v1.2.3) (2022-09-23)


### Bug Fixes

* style to trigger build ([618a375](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/618a375ab40e157837cff43d20b84e2bc14ff510))

## [1.2.2](https://github.com/informatievlaanderen/basisregisters-aws-lambda/compare/v1.2.1...v1.2.2) (2022-09-23)

## [1.2.1](https://github.com/informatievlaanderen/basisregisters-aws-lambda/compare/v1.2.0...v1.2.1) (2022-09-08)

# [1.2.0](https://github.com/informatievlaanderen/basisregisters-aws-lambda/compare/v1.1.0...v1.2.0) (2022-07-22)


### Features

* add Logger to MessageMetadata ([3019f2c](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/3019f2c7e5e1accfee657e8eece0b5eb636d4a64))

# [1.1.0](https://github.com/informatievlaanderen/basisregisters-aws-lambda/compare/v1.0.0...v1.1.0) (2022-06-29)


### Features

* add metadata ([a765aed](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/a765aed5430c1af66cc5aaf541770e4c94ecc810))

# 1.0.0 (2022-06-20)


### Bug Fixes

* add npm ([24758f1](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/24758f1a894baf470994047fbb80fd780bb68a68))
* remove duplicate nuget push ([785de52](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/785de523b3053ed6111c438a4323103d1b6045e0))
* update package.json ([b4fd2ab](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/b4fd2abbd49cf37c9df2999b9a2ab0d5d7b29587))


### Features

* initial commit ([#10](https://github.com/informatievlaanderen/basisregisters-aws-lambda/issues/10)) ([20eeaa3](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/20eeaa39cda9719b51e2acd737e4cdedc052003e))
* initial commit ([#11](https://github.com/informatievlaanderen/basisregisters-aws-lambda/issues/11)) ([23b26cf](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/23b26cfe53ec1e872a25cb7f75e4dbd185b02fc8))
* initial commit ([#9](https://github.com/informatievlaanderen/basisregisters-aws-lambda/issues/9)) ([9604593](https://github.com/informatievlaanderen/basisregisters-aws-lambda/commit/96045931b5e02a595d748f392247c3e87da9d206))
