# I am completely unable to get referential paths to work in this project with my volume mounts
# I am guessing it has to do with my docker toolbox limitations.

FROM stefanwalther/mongo-seed

COPY ./people.json /data/people.json

CMD "mongoimport --host mongo --port 27017 --db FunctionsTest --collection Person --mode upsert --type json --file /data/people.json --jsonArray" 