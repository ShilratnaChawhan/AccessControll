# pull official base image
FROM node:18 as build

#working directory of containerized app
WORKDIR /app

#copy 
COPY . /app/

#prepare the container for building 
RUN npm install

# RUN npm install 
RUN npm run build

#prepare nginx
FROM nginx:1.16.0-alpine
COPY --from=build /app/dist/access-controll-ui /usr/share/nginx/html 

RUN rm /etc/nginx/conf.d/default.conf

COPY nginx/nginx.conf /etc/nginx/conf.d

#fire for nginx
EXPOSE 80

CMD [ "nginx","-g","daemon off;" ]

