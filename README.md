[![Build Status](https://barradas.visualstudio.com/Contributions/_apis/build/status/DockerImage/RedisMigrate%20Worker?branchName=master)](https://barradas.visualstudio.com/Contributions/_build/latest?definitionId=22&branchName=master)

# RedisMigrate
 
A simple worker to copy Redis values to another Redis filtered and with prefix;

## Running with Docker

```
docker run --name redis-migrate -d \
    -e MaxThreads=1000 \
    -e OriginConnectionString="localhost:6379,password=RedisAuth,defaultDatabase=0,syncTimeout=2000,connectTimeout=2000,abortConnect=false" \
    -e OriginDatabase=0 \
    -e OriginFilter="*" \
    -e OriginPopulateEnabled=false \
    -e OriginPopulateQuantity=10000 \
    -e OriginPopulatePrefix="some-test:" \
    -e DestinationConnectionString="localhost:6377,password=RedisAuth,defaultDatabase=0,syncTimeout=2000,connectTimeout=2000,abortConnect=false" \
    -e DestinationDatabase=0 \
    -e DestinationKeyPrefix="migrated:" \
    -e DestinationKeyReplaceForEmpty="some-test:" \
    -e DestinationReplace=true \
    thiagobarradas/redis-migrate:latest
```

## Configuration

Set environment variables to setup RedisMigrate:

| Env Var | Type | Required | Description | e.g. |
| ------- | ---- | -------- | ----------- | ---- |
| `MaxThreads`             | integer | no | parallel tasks number - min 50 | `100` |
| `OriginConnectionString` | string | yes | origin redis connection | `localhost:6379,password=RedisAuth,defaultDatabase=0,syncTimeout=2000,connectTimeout=2000,abortConnect=false` |
| `OriginDatabase`             | integer | yes | origin database number | `0` |
| `OriginFilter`             | string | yes | filter to get keys from origin | `*` |
| `DestinationConnectionString` | string | yes | origin redis connection | `localhost:6377,password=RedisAuth,defaultDatabase=0,syncTimeout=2000,connectTimeout=2000,abortConnect=false` |
| `DestinationDatabase`             | integer | yes | origin database number | `0` |
| `DestinationKeyPrefix`             | string | no | prefix to add into key for destination | `migrated:` |
| `DestinationKeyReplaceForEmpty`             | string | no | string to remove from key | `something` |
| `DestinationReplace`             | boolean | no | if true, makes a upsert into destination | `true` |
| `OriginPopulateEnabled`             | bool | no | only for test - after execution, insert some data into origin | `false` |
| `OriginPopulateQuantity`             | string | no | only for test - number of entries writed previously | `1000` |
| `OriginPopulatePrefix`             | string | no | only for test - prefix to apply into test data | `some-test:` |

## How can I contribute?

Please, refer to [CONTRIBUTING](https://github.com/ThiagoBarradas/redis-migrate/edit/master/.github/CONTRIBUTING.md)

## Found something strange or need a new feature?

Open a new Issue following our issue template [ISSUE TEMPLATE](https://github.com/ThiagoBarradas/redis-migrate/edit/master/.github/ISSUE_TEMPLATE.md)

## Did you like it? Please, make a donate :)

if you liked this project, please make a contribution and help to keep this and other initiatives, send me some Satochis.

BTC Wallet: `1G535x1rYdMo9CNdTGK3eG6XJddBHdaqfX`

![1G535x1rYdMo9CNdTGK3eG6XJddBHdaqfX](https://i.imgur.com/mN7ueoE.png)
