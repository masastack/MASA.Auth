async function UploadImage(imageFiles, ossParamter) {
    console.log("UploadImage", ossParamter);
    const client = new OSS(ossParamter);

    const headers = {
        // ָ����Object������ʱ��ҳ�Ļ�����Ϊ��
        // 'Cache-Control': 'no-cache',
        // ָ����Object������ʱ�����ơ�
        // 'Content-Disposition': 'oss_download.txt',
        // ָ����Object������ʱ�����ݱ����ʽ��
        // 'Content-Encoding': 'UTF-8',
        // ָ������ʱ�䡣
        // 'Expires': 'Wed, 08 Jul 2022 16:57:01 GMT',
        // ָ��Object�Ĵ洢���͡�
        // 'x-oss-storage-class': 'Standard',
        // ָ��Object�ķ���Ȩ�ޡ�
        'x-oss-object-acl': 'public-read-write',
        // ����Object�ı�ǩ����ͬʱ���ö����ǩ��
        // 'x-oss-tagging': 'Tag1=1&Tag2=2',
        // ָ��CopyObject����ʱ�Ƿ񸲸�ͬ��Ŀ��Object���˴�����Ϊtrue����ʾ��ֹ����ͬ��Object��
        // 'x-oss-forbid-overwrite': 'true',
    };

    return await putObject(client, imageFiles[0], headers);
}

async function putObject(client, file, headers) {
    try {
        // ��дObject����·����Object����·���в��ܰ���Bucket���ơ�
        // ������ͨ���Զ����ļ���������exampleobject.txt�����ļ�����·��������exampledir/exampleobject.txt������ʽʵ�ֽ������ϴ�����ǰBucket��Bucket�е�ָ��Ŀ¼��
        // data��������Զ���Ϊfile����Blob���ݻ���OSS Buffer��
        const result = await client.put(
            `${file.name}`,
            file,
            {
               headers
            }
        );
        console.log(result);
        return [result.url];
    } catch (e) {
        console.log(e);
    }
}