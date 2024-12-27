const getToday = () => {
  const date = new Date();
  return date.toISOString().slice(0, 10);
};


const getTimeAgo = (dateCreate: string): string => {
  const currentDate = new Date();
  const createdDate = new Date(dateCreate);
  const diffInMs = currentDate.getTime() - createdDate.getTime();
  const diffInMinutes = Math.floor(diffInMs / (1000 * 60));
  const diffInHours = Math.floor(diffInMs / (1000 * 60 * 60));
  const diffInDays = Math.floor(diffInMs / (1000 * 60 * 60 * 24));

  if (diffInDays >= 1) {
    return `${diffInDays} ngày trước`;
  } else if (diffInHours >= 1) {
    return `${diffInHours} giờ trước`;
  } else if (diffInMinutes >= 1) {
    return `${diffInMinutes} phút trước`;
  } else {
    return "Vừa xong";
  }
};
export { getToday , getTimeAgo};
