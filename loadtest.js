import http from 'k6/http';

export let options = {
  vus: 5000,           // تعداد کاربران همزمان
  duration: '3s',      // مدت تست
};

export default function () {
      http.get("http://host.docker.internal:8080/"); 
}
